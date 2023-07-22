
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
namespace OneSDK
{
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public class BasePathFinder<T> : ScriptableObject where T : ScriptableObject
    {
        private static bool debugMode => false;
        private static string s_foundPath = string.Empty;
        public static string pathInAssets
        {
            get
            {
#if UNITY_EDITOR
				if (!string.IsNullOrEmpty(s_foundPath))
				{
					if (debugMode) Debugger.Log($"Saved Path: {s_foundPath}");
					return s_foundPath;
				}

				T obj = CreateInstance<T>();
				var s = UnityEditor.MonoScript.FromScriptableObject(obj);
				string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
				DestroyImmediate(obj);
				var fileInfo = new FileInfo(assetPath);
				DirectoryInfo baseDir = fileInfo.Directory;
				Debug.Assert(baseDir != null, nameof(baseDir) + " != null");
				string baseDirPath = CleanPath(baseDir.ToString());
				int index = baseDirPath.IndexOf("Assets/", StringComparison.Ordinal);
				Assert.IsTrue(index >= 0);
				baseDirPath = baseDirPath.Substring(index);
				s_foundPath = baseDirPath;
				if (debugMode) Debugger.Log($"Found Path: {s_foundPath}");
				return baseDirPath;
#else
                return "Path cannot be returned outside the Unity Editor";
#endif
            }
        }
        public static string path
        {
            get

            {
#if UNITY_EDITOR
				if (!string.IsNullOrEmpty(s_foundPath))
				{
					if (debugMode) Debugger.Log($"Saved Path: {s_foundPath}");
					return s_foundPath;
				}

				T obj = CreateInstance<T>();
				var s = UnityEditor.MonoScript.FromScriptableObject(obj);
				string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
				DestroyImmediate(obj);
				var fileInfo = new FileInfo(assetPath);
				DirectoryInfo baseDir = fileInfo.Directory;
				Debug.Assert(baseDir != null, nameof(baseDir) + " != null");
				string baseDirPath = CleanPath(baseDir.ToString());
				
				s_foundPath = baseDirPath;
				if (debugMode) Debugger.Log($"Found Path: {s_foundPath}");
				return baseDirPath;
#else
                return "Path cannot be returned outside the Unity Editor";
#endif
            }
        }

        private static string CleanPath(string rawPath) =>
            rawPath.Replace('\\', '/');
    }
}