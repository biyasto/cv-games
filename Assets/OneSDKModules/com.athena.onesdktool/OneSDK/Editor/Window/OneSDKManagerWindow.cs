using UnityEngine;
using UnityEditor;

namespace OneSDK.Editor
{
    public class OneSDKManagerWindow : EditorWindow
    {
        #region Config

        //==============================================
        // Window config
        //==============================================
        private const string windowTitle = "One SDK Tool";
        private static readonly Vector2 windowMinSize = new Vector2(520, 200);

        //==============================================
        // Drawing config
        //==============================================
        private const float actionFieldWidth = 60f;
        private static readonly GUILayoutOption fieldWidth = GUILayout.Width(actionFieldWidth);
        private const float networkFieldMinWidth = 150f;
        private static GUILayoutOption networkWidthOption = GUILayout.MinWidth(networkFieldMinWidth);
        private const float versionFieldMinWidth = 100f;
        private static GUILayoutOption versionWidthOption = GUILayout.MinWidth(versionFieldMinWidth);

        private GUIStyle titleLabelStyle;
        private GUIStyle headerLabelStyle;
        private GUIStyle wrapTextLabelStyle;

        #endregion

        private PluginManifest pluginManifest;
        private PluginManifest pluginManifestLocal;
        private bool pluginManifestLocalLoadFailed;
        private bool pluginManifestLoadFailed;

        private float previousWindowWidth;
        private Vector2 scrollPosition;


        public static void ShowSDKManager()
        {
            OneSDKManagerWindow window = EditorWindow.GetWindow<OneSDKManagerWindow>(true, windowTitle, true);
            window.minSize = windowMinSize;
            window.Show();
        }

        private void Awake()
        {
            SetupDrawStyles();
        }

        private void OnEnable()
        {
            Load();
        }

        //==============================================
        // Init
        //==============================================
        private void Load()
        {
            pluginManifestLoadFailed = pluginManifest == null;
            pluginManifest = OneSDKManager.Instance.LoadPluginManifest();

        }

        private void SetupDrawStyles()
        {
            titleLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 20
            };

            headerLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18
            };

            wrapTextLabelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
        }

        //==============================================
        // Drawing
        //==============================================
        private void OnGUI()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = scrollView.scrollPosition;
                DrawPlugins();
            }

        }

        private void DrawPlugins()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                DrawPluginDetailHeaders();

                // Immediately after downloading and importing a plugin the entire IDE reloads and current versions can be null in that case. Will just show loading text in that case.
                if (pluginManifest != null)
                {
                    var plugins = pluginManifest.Plugins;
                    foreach (PluginInfo plugin in plugins)
                    {
                        DrawPluginDetailRow(plugin);
                    }

                    GUILayout.Space(5);
                }
                else
                {
                    DrawEmptyPluginManifest();
                }
            }

            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }

        private void DrawPluginDetailHeaders()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Plugin", headerLabelStyle, networkWidthOption);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Current Version", headerLabelStyle, versionWidthOption);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Lasted Version", headerLabelStyle, versionWidthOption);
                GUILayout.FlexibleSpace();
                GUILayout.Button("Actions", headerLabelStyle, fieldWidth);
                GUILayout.Space(5);
            }

            GUILayout.Space(4);
        }
        private void DrawTitleSaveProgressSDK(PluginInfo plugin)
        {
            //seprate SDK - should more info
            if (plugin.Id.Equals("save_progress"))
            {
                GUILayout.Space(15);
                EditorGUILayout.LabelField("Save progress SDK", titleLabelStyle);
                GUILayout.Space(5);
            }
        }
        private void DrawTitleTrackUAIfNeeded(PluginInfo plugin)
        {
            //seprate SDK - should more info
            if (plugin.Id.Equals("athena_att_package"))
            {
                GUILayout.Space(15);
                EditorGUILayout.LabelField("ATT Tracking", titleLabelStyle);
                GUILayout.Space(5);
            }
        }
        private void DrawTitleOneSDKModuleIfNeeded(PluginInfo plugin)
        {
            if (plugin.Id.Equals("com.athena.onesdktool.iap-system"))
            {
                GUILayout.Space(15);
                EditorGUILayout.LabelField("[One SDK Module]", titleLabelStyle);
                GUILayout.Space(5);
            }
        }

        private void DrawPluginDetailRow(PluginInfo plugin)
        {
            string action;
            bool isInstalling;
            switch (plugin.PluginState)
            {
                case PluginState.NotInstalled:
                    action = "Install";
                    isInstalling = true;
                    break;
                case PluginState.Installed:
                    action = "Remove";
                    isInstalling = false;
                    break;
                case PluginState.NeedUpdate:
                    action = "Upgrade";
                    isInstalling = true;
                    break;
                default:
                    action = "Install";
                    isInstalling = true;
                    break;
            }
            DrawTitleSaveProgressSDK(plugin);
            DrawTitleTrackUAIfNeeded(plugin);
            DrawTitleOneSDKModuleIfNeeded(plugin);
            GUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandHeight(false)))
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField(new GUIContent(plugin.Name), networkWidthOption);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(new GUIContent(plugin.PluginState == PluginState.NotInstalled ? "None" : plugin.LocalVersion), versionWidthOption);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(new GUIContent(plugin.Version), versionWidthOption);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent(action), fieldWidth))
                {
                    if (isInstalling)
                    {
                        OneSDKEditorCoroutine.StartCoroutine(
                            OneSDKManager.Instance.InstallPlugin(plugin, ShowDownloadProcess)
                        );
                    }
                    else if (EditorUtility.DisplayDialog(
                        "Remove Plugin",
                        string.Format("Are you sure you want to remove plugin {0}?", plugin.Name),
                        "Remove",
                        "Cancel"))
                    {
                        OneSDKManager.Instance.RemovePlugin(plugin);
                    }
                }

                GUI.enabled = true;
                GUILayout.Space(5);
            }
        }

        private void DrawEmptyPluginManifest()
        {
            GUILayout.Space(5);

            // Plugin manifest failed to load. Show error and retry button.
            if (pluginManifestLoadFailed)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);

                EditorGUILayout.LabelField("Failed to load plugin manifest. Please click Retry or restart the manager.", titleLabelStyle);
                if (GUILayout.Button("Retry", fieldWidth))
                {
                    pluginManifestLoadFailed = false;
                    Load();
                }

                GUILayout.Space(5);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            // Still loading, show loading label.
            else
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("Loading...", titleLabelStyle);

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            GUILayout.Space(5);
        }

        private void ShowDownloadProcess(string status, float progress, bool done)
        {
            // Download is complete. Clear progress bar.
            if (done)
            {
                EditorUtility.ClearProgressBar();
            }
            // Download is in progress, update progress bar.
            else
            {
                if (EditorUtility.DisplayCancelableProgressBar(windowTitle, status, progress))
                {
                    OneSDKManager.Instance.CancelInstall();
                    EditorUtility.ClearProgressBar();
                }
            }
        }
    }
}
