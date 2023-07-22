using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using OneSDK;
namespace OneSDK.Editor
{
    public class OneSDKManager
    {
        // Delegate to be called when downloading a plugin with the progress percentage. 
        public delegate void DownloadPluginProgressCallback(string status, float progress, bool done);

        public static readonly OneSDKManager Instance = new OneSDKManager();

        public string BaseEditorPath = EditorPath.path;
        public const string LocalPluginInfoPath = "Assets/Resources";
        public const string PluginManifestLocalFileName = "plugin-manifest-local.json";
        public const string PluginManifestFileName = "plugin-manifest.json";

        public PluginManifest PluginManifest => pluginManifest;
        private PluginManifest pluginManifest;
        public PluginManifest PluginManifestLocal => pluginManifestLocal;
        private PluginManifest pluginManifestLocal;
        private UnityWebRequest downloadPluginRequest;
        private AddRequest _addRequest;
        private RemoveRequest _removeRequest;
        private PluginInfo importingPlugin;

        private OneSDKManager()
        {
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageStarted -= OnImportPackageStarted;
            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;

            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageStarted += OnImportPackageStarted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
            AssetDatabase.importPackageCancelled += OnImportPackageCancelled;
        }

        ~OneSDKManager()
        {
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageStarted -= OnImportPackageStarted;
            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
        }
        public PluginManifest LoadPluginManifest()
        {
            try
            {
                string path = Path.Combine(BaseEditorPath, "Resources", PluginManifestFileName);
                string json = File.ReadAllText(path);
                pluginManifest = JsonUtility.FromJson<PluginManifest>(json);

                //Local manifest
                string pathLocalPlugin = Path.Combine(LocalPluginInfoPath, PluginManifestLocalFileName);
                if (!File.Exists(pathLocalPlugin))
                {
                    if (!Directory.Exists(LocalPluginInfoPath))
                    {
                        Directory.CreateDirectory(LocalPluginInfoPath);
                    }
                    else
                    {
                        FileUtil.CopyFileOrDirectory(path, pathLocalPlugin);
                    }

                }
                else
                {
                    //json local
                    string jsonLocal = File.ReadAllText(pathLocalPlugin);
                    pluginManifestLocal = JsonUtility.FromJson<PluginManifest>(jsonLocal);
                    List<PluginInfo> newPlugins = new List<PluginInfo>();
                    if (pluginManifest != null && pluginManifestLocal != null)
                    {
                        var pluginLocals = pluginManifestLocal.Plugins;
                        foreach (var plugin in pluginManifest.Plugins)
                        {
                            if (pluginLocals != null)
                            {
                                PluginInfo localPlugin = pluginLocals.FirstOrDefault(x => x.Id == plugin.Id);
                                if (localPlugin != null)
                                {
                                    plugin.LocalVersion = localPlugin.Version;
                                }
                                else
                                {
                                    newPlugins.Add(plugin);
                                }
                            }
                            RefreshPluginInstallStatus(plugin);
                        }
                        pluginManifestLocal.Plugins.AddRange(newPlugins);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[OneSDK] Failed to load Athena SDK Manifest: " + e);
                pluginManifest = null;
            }

            return pluginManifest;
        }

        public PluginManifest UpdateLocalPluginManifest(PluginInfo plugin)
        {
            try
            {
                //json update
                string pathLocal = Path.Combine(LocalPluginInfoPath, PluginManifestLocalFileName);

                if (pluginManifestLocal != null)
                {
                    var localPlugin = pluginManifestLocal.Plugins.FirstOrDefault<PluginInfo>(x => x.Id == plugin.Id);
                    if (localPlugin != null && plugin != null)
                    {
                        plugin.LocalVersion = plugin.Version;
                        localPlugin.UpdatePluginInfo(plugin);
                    }
                    else
                    {
                        plugin.LocalVersion = plugin.Version;
                        pluginManifestLocal.Plugins.Add(plugin);
                    }
                    string jsonString = JsonUtility.ToJson(pluginManifestLocal);
                    File.WriteAllText(pathLocal, jsonString);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[OneSDK] Failed to load Athena SDK Manifest: " + e);
                pluginManifest = null;
            }


            return pluginManifest;
        }
        //==============================================
        // Plugin Management
        //==============================================
        public IEnumerator InstallPlugin(
            PluginInfo plugin,
            DownloadPluginProgressCallback downloadProgressCallback = null)
        {
            if (plugin.PluginType == PluginType.UnityPackage)
            {
                if (downloadPluginRequest != null) yield return null;
                var path = Path.Combine(Application.temporaryCachePath, GetPluginFileName(plugin));
                var downloadHandler = new DownloadHandlerFile(path);
                downloadPluginRequest = new UnityWebRequest(plugin.DownloadUrl) { method = UnityWebRequest.kHttpVerbGET, downloadHandler = downloadHandler };
                string status = $"Downloading {plugin.Name}";
                UnityWebRequestAsyncOperation operation = downloadPluginRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    yield return new WaitForSeconds(0.1f);
                    downloadProgressCallback?.Invoke(status, operation.progress, operation.isDone);
                }

#if UNITY_2020_1_OR_NEWER
            if (downloadPluginRequest.result != UnityWebRequest.Result.Success)
#elif UNITY_2017_2_OR_NEWER
            if (downloadPluginRequest.isNetworkError || downloadPluginRequest.isHttpError)
#else
                if (downloadPluginRequest.isError)
#endif
                {
                    if (!downloadPluginRequest.error.Contains("aborted"))
                    {
                        Debug.LogError("[OneSDK] Failed to download plugin: " + downloadPluginRequest.error);
                    }
                }
                else
                {
                    importingPlugin = plugin;
                    AssetDatabase.ImportPackage(path, true);
                }
                downloadPluginRequest = null;
            }
            else if (plugin.PluginType == PluginType.Git)
            {
                importingPlugin = plugin;
                _addRequest = Client.Add(plugin.DownloadUrl);
                EditorApplication.update += AddRequestProgress;
            }

        }

        public void CancelInstall()
        {
            downloadPluginRequest?.Abort();
            EditorUtility.ClearProgressBar();
            downloadPluginRequest = null;
        }

        public void RemovePlugin(PluginInfo plugin)
        {
            if (plugin.PluginType == PluginType.UnityPackage)
            {
                if (plugin.ContentManifest == null || plugin.ContentManifest.Length <= 0)
                {
                    Debug.LogWarningFormat("[OneSDK] Failed to remove plugin {0} because it has no content manifest.", plugin.Name);
                    return;
                }
                string[] contentManifest = plugin.ContentManifest;
                foreach (string contentPath in plugin.ContentManifest)
                {
                    string path = Path.Combine(Application.dataPath, contentPath);
                    if (Directory.Exists(path) || File.Exists(path))
                    {
                        FileUtil.DeleteFileOrDirectory(path);
                        File.Delete(path + ".meta");
                    }
                }
                plugin.PluginState = PluginState.NotInstalled;
                AssetDatabase.Refresh();
            }
            else if (plugin.PluginType == PluginType.Git)
            {
                _removeRequest = Client.Remove(plugin.Id);
                EditorApplication.update += OnPackageRemoveProgress;
            }
        }
        private void AddRequestProgress()
        {
            if (_addRequest != null)
            {
                if (_addRequest.IsCompleted)
                {
                    if (_addRequest.Status == StatusCode.Success)
                    {
                        UpdateLocalPluginManifest(importingPlugin);
                        AssetDatabase.Refresh();
                        RefreshPluginInstallStatus(importingPlugin);
                        Debugger.Log("Installed: " + _addRequest.Result.packageId);
                    }
                    else if (_addRequest.Status >= StatusCode.Failure)
                        Debugger.Log(_addRequest.Error.message);
                    _addRequest = null;
                    importingPlugin = null;
                    EditorApplication.update -= AddRequestProgress;
                }
            }
        }
        private void OnPackageRemoveProgress()
        {
            if (_removeRequest.IsCompleted)
            {
                if (_removeRequest.Status == StatusCode.Success)
                {
                    var plugins = pluginManifest.Plugins;
                    var plugin = plugins.FirstOrDefault<PluginInfo>(x => x.Id == _removeRequest.PackageIdOrName);
                    if (plugin != null)
                    {
                        plugin.PluginState = PluginState.NotInstalled;
                        AssetDatabase.Refresh();
                    }
                    Debugger.Log("Removed: " + _removeRequest.PackageIdOrName);

                }
                else if (_removeRequest.Status >= StatusCode.Failure)
                    Debugger.Log(_removeRequest.Error.message);

                EditorApplication.update -= OnPackageRemoveProgress;
                _removeRequest = null;

            }
        }

        private void RefreshPluginInstallStatus(PluginInfo plugin)
        {
            if (plugin.LocalVersion.IsNullOrEmpty())
            {
                plugin.PluginState = PluginState.NotInstalled;
                return;
            }


            bool installed = true;
            if (plugin.PluginType == PluginType.UnityPackage)
            {
                if (plugin.ContentManifest == null || plugin.ContentManifest.Length <= 0)
                {
                    Debug.LogWarningFormat("[OneSDK] Failed to check install status for plugin {0} because it has no content manifest.", plugin.Name);
                    return;
                }
                string[] contentManifest = plugin.ContentManifest;
                foreach (string contentPath in plugin.ContentManifest)
                {
                    string path = Path.Combine(Application.dataPath, contentPath);
                    installed &= Directory.Exists(path) || File.Exists(path);
                    if (!installed)
                    {
                        break;
                    }
                }
            }
            else if (plugin.PluginType == PluginType.Git)
            {
                installed = Directory.Exists($"Packages/{plugin.Id}");
            }

            plugin.PluginState = installed ? PluginState.Installed : PluginState.NotInstalled;

            if (plugin.LocalVersion != null && OneSdkToolUtils.CompareVersions(plugin.LocalVersion, plugin.Version) == OneSdkToolUtils.VersionComparisonResult.Lesser)
            {
                if (plugin.PluginState == PluginState.Installed)
                {
                    plugin.PluginState = PluginState.NeedUpdate;
                }
            }
        }

        //==============================================
        // Import package callbacks
        //==============================================
        private void OnImportPackageCompleted(string packageName)
        {
            var plugin = pluginManifest.Plugins.FirstOrDefault<PluginInfo>(x => packageName.Contains(x.Id));
            UpdateLocalPluginManifest(plugin);
            AssetDatabase.Refresh();
            RefreshPluginInstallStatus(plugin);
            importingPlugin = null;
        }

        private void OnImportPackageCancelled(string packageName)
        {
            Debugger.LogError("[OneSDK] Cancelled  import plugin: " + packageName);
            importingPlugin = null;
        }

        private void OnImportPackageCancelled(string packageName, string errorMessage)
        {
            Debugger.LogError("[OneSDK] Cancelled import plugin: " + errorMessage);
            importingPlugin = null;
        }




        private static void OnImportPackageFailed(string packagename, string errormessage)
        {
            Debugger.Log($"Failed importing package: {packagename} with error: {errormessage}");
        }

        private static void OnImportPackageStarted(string packagename)
        {
            Debugger.Log($"Started importing package: {packagename}");
        }

        private bool IsImportingPlugin(string packageName)
        {
            // Note: The packageName doesn't have the '.unitypacakge' extension included in its name but the file name does. So using Contains instead of Equals.
            return importingPlugin != null && GetPluginFileName(importingPlugin).Contains(packageName);
        }

        //==============================================
        // Misc
        //==============================================
        private static string GetPluginFileName(PluginInfo info)
        {
            return string.Format("{0}_{1}.unitypackage", info.Id, info.Version);
        }
    }
}
