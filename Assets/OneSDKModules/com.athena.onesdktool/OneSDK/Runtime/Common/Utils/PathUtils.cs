
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OneSDK
{
    public static class PathUtils
    {
        public static void CreatePath(string path)
        {
            Directory.CreateDirectory(ToAbsolutePath(path));
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);
            #endif
        }

        public static string CleanPath(string path) =>
            path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Replace($"{Path.AltDirectorySeparatorChar}{Path.AltDirectorySeparatorChar}", Path.AltDirectorySeparatorChar.ToString());

        public static string ToAbsolutePath(string path) =>
            CleanPath
            (
                path.Contains(Application.dataPath)
                    ? path
                    : path.RemoveFirst("Assets".Length).AppendPrefixIfMissing(Application.dataPath)
            );

        public static bool IsAbsolutePath(string path) =>
            path.StartsWith(Application.dataPath);

        public static string ToRelativePath(string path) =>
            CleanPath
            (
                IsAbsolutePath(path)
                    ? "Assets" + path.Substring(Application.dataPath.Length)
                    : path
            );

        public static bool IsRelativePath(string path) =>
            !IsAbsolutePath(path);

        public static string[] GetResourcesDirectories()
        {
            var result = new List<string>();
            var stack = new Stack<string>();
            stack.Push(Application.dataPath); // Add the root directory to the stack
            while (stack.Count > 0)           // While we have directories to process...
            {
                string currentDir = stack.Pop(); // Grab a directory off the stack
                try
                {
                    foreach (string dir in Directory.GetDirectories(currentDir))
                    {
                        if (Path.GetFileName(dir).Equals("Resources"))
                            result.Add(dir); // If one of the found directories is a Resources dir, add it to the result
                        stack.Push(dir);     // Add directories at the current level into the stack
                    }
                }
                catch
                {
                    Debugger.LogError($"Directory {currentDir} couldn't be read from");
                }
            }
            return result.ToArray();
        }

        public static bool PathIsDirectory(string path) =>
            (File.GetAttributes(ToAbsolutePath(path)) & FileAttributes.Directory) == FileAttributes.Directory;

        public static string GetDirectoryName(string path) =>
            CleanPath(Path.GetDirectoryName(path));

        public static string GetFileName(string path) =>
            CleanPath(Path.GetFileName(path));

        public static string GetFileNameWithoutExtension(string path) =>
            CleanPath(Path.GetFileNameWithoutExtension(path));

        public static string GetExtension(string path) =>
            CleanPath(Path.GetExtension(path));

        public static bool HasExtension(string path) =>
            Path.HasExtension(CleanPath(path));
    }
}
