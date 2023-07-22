using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
namespace OneSDK.Editor
{
    public class OneSdkToolUtils
    {
        /// <summary>
        /// An Enum to be used when comparing two versions.
        ///
        /// If:
        ///     A &lt; B    return <see cref="Lesser"/>
        ///     A == B      return <see cref="Equal"/>
        ///     A &gt; B    return <see cref="Greater"/>
        /// </summary>
        public enum VersionComparisonResult
        {
            Lesser = -1,
            Equal = 0,
            Greater = 1
        }

        /// <summary>
        /// Compares its two arguments for order.  Returns <see cref="VersionComparisonResult.Lesser"/>, <see cref="VersionComparisonResult.Equal"/>,
        /// or <see cref="VersionComparisonResult.Greater"/> as the first version is less than, equal to, or greater than the second.
        /// </summary>
        /// <param name="versionA">The first version to be compared.</param>
        /// <param name="versionB">The second version to be compared.</param>
        /// <returns>
        /// <see cref="VersionComparisonResult.Lesser"/> if versionA is less than versionB.
        /// <see cref="VersionComparisonResult.Equal"/> if versionA and versionB are equal.
        /// <see cref="VersionComparisonResult.Greater"/> if versionA is greater than versionB.
        /// </returns>
        public static VersionComparisonResult CompareVersions(string versionA, string versionB)
        {
            if (versionA.IsNullOrEmpty() || versionB.IsNullOrEmpty())
            {
                return VersionComparisonResult.Equal;
            }

            if (versionA.Equals(versionB)) return VersionComparisonResult.Equal;

            // Check if either of the versions are beta versions. Beta versions could be of format x.y.z-beta or x.y.z-betaX.
            // Split the version string into beta component and the underlying version.
            int piece;
            var isVersionABeta = versionA.Contains("-beta");
            var versionABetaNumber = 0;
            if (isVersionABeta)
            {
                var components = versionA.Split(new[] { "-beta" }, StringSplitOptions.None);
                versionA = components[0];
                versionABetaNumber = int.TryParse(components[1], out piece) ? piece : 0;
            }

            var isVersionBBeta = versionB.Contains("-beta");
            var versionBBetaNumber = 0;
            if (isVersionBBeta)
            {
                var components = versionB.Split(new[] { "-beta" }, StringSplitOptions.None);
                versionB = components[0];
                versionBBetaNumber = int.TryParse(components[1], out piece) ? piece : 0;
            }

            // Now that we have separated the beta component, check if the underlying versions are the same.
            if (versionA.Equals(versionB))
            {
                // The versions are the same, compare the beta components.
                if (isVersionABeta && isVersionBBeta)
                {
                    if (versionABetaNumber < versionBBetaNumber) return VersionComparisonResult.Lesser;

                    if (versionABetaNumber > versionBBetaNumber) return VersionComparisonResult.Greater;
                }
                // Only VersionA is beta, so A is older.
                else if (isVersionABeta)
                {
                    return VersionComparisonResult.Lesser;
                }
                // Only VersionB is beta, A is newer.
                else
                {
                    return VersionComparisonResult.Greater;
                }
            }

            // Compare the non beta component of the version string.
            var versionAComponents = versionA.Split('.').Select(version => int.TryParse(version, out piece) ? piece : 0).ToArray();
            var versionBComponents = versionB.Split('.').Select(version => int.TryParse(version, out piece) ? piece : 0).ToArray();
            var length = Mathf.Max(versionAComponents.Length, versionBComponents.Length);
            for (var i = 0; i < length; i++)
            {
                var aComponent = i < versionAComponents.Length ? versionAComponents[i] : 0;
                var bComponent = i < versionBComponents.Length ? versionBComponents[i] : 0;

                if (aComponent < bComponent) return VersionComparisonResult.Lesser;

                if (aComponent > bComponent) return VersionComparisonResult.Greater;
            }
            return VersionComparisonResult.Equal;
        }

        #region App Config Helper
        public static Dictionary<string, Dictionary<string, ConfigValue>> LoadAppConfig(byte[] data)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(new System.IO.MemoryStream(data)))
            {
                return LoadAppConfig(reader);
            }


        }

        public static Dictionary<string, Dictionary<string, ConfigValue>> LoadAppConfig(string iniFile)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(iniFile))
            {
                return LoadAppConfig(reader);
            }
        }

        public static ConfigValue GetValue(Dictionary<string, Dictionary<string, ConfigValue>> sections, string key, string section)
        {
            return sections[section][key];
        }

        private static Dictionary<string, Dictionary<string, ConfigValue>> LoadAppConfig(System.IO.StreamReader reader)
        {
            Dictionary<string, Dictionary<string, ConfigValue>> sections = new Dictionary<string, Dictionary<string, ConfigValue>>();
            string section = null;
            Dictionary<string, ConfigValue> attributes = null;

            string line;
            int count = 0;
            while ((line = reader.ReadLine()) != null)
            {
                count++;

                if (line.Length == 0)
                    continue;

                if (line[0] == ' ' || line[0] == '\t' || line[line.Length - 1] == ' ' || line[line.Length - 1] == '\t')
                    line = line.Trim();

                if (line[0] == ';')
                    continue;

                if (line[0] == '[')
                {
                    if (line[line.Length - 1] != ']')
                    {
                        Debug.LogErrorFormat("Parse configs failed at line {0}. Expect \']\'!", count);
                        sections.Clear();
                        return null;
                    }

                    section = line.Substring(1, line.Length - 2);
                    attributes = new Dictionary<string, ConfigValue>();
                    sections.Add(section, attributes);
                    continue;
                }

                if (section == null)
                {
                    Debug.LogErrorFormat("Parse configs failed at line {0}. Section not found!", count);
                    return null;
                }

                // parse key & value
                var splitIdx = line.IndexOf('=');
                if (splitIdx < 0)
                {
                    Debug.LogErrorFormat("Parse configs failed at line {0}. Key and value not found!", count);
                    sections.Clear();
                    return null;
                }

                var keyIdx = splitIdx - 1;
                var valueIdx = splitIdx + 1;

                while (line[keyIdx] == ' ' || line[keyIdx] == '\t')
                {
                    keyIdx--;
                    continue;
                }
                var key = line.Substring(0, keyIdx + 1);

                var value = string.Empty;
                if (valueIdx < line.Length)
                {
                    while (line[valueIdx] == ' ' || line[valueIdx] == '\t')
                    {
                        valueIdx++;
                        continue;
                    }
                    value = line.Substring(valueIdx, line.Length - valueIdx);
                }

                var configValue = new ConfigValue(value);
                attributes.Add(key, configValue);
            }
            return sections;
        }
#if UNITY_IOS
        public static void LoadPlistDocument(string path) {
            var plistDocument = new PlistDocument();
			plistDocument.ReadFromFile(path);
        }
#endif
        public static void SaveObjectToFile(object data, string folderPath, string fileName)
        {
            try
            {
                string filePath = Path.Combine(folderPath, fileName);
                Directory.CreateDirectory(folderPath);
                // Debug.Log($"--- Save file: {fileName}");
                // JsonConvert.SerializeObject(data, new JsonSerializerSettings
                // {
                //     TypeNameHandling = TypeNameHandling.Auto,
                //     TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                // });
                List<string> contents = new List<string>();

                Type myType = data.GetType();
                var props = myType.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    object propValue = prop.GetValue(data, null);
                    var propName = $"{prop.Name}:";
                    contents.Add( $"{FixedLength(propName, 38)}{propValue.ToString()}");
                }
                File.WriteAllLines(filePath, contents);

            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        public static byte[] LoadFromFileToBytes(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            return File.ReadAllBytes(filePath);
        }

        public static string FixedLength(string str, int rangeAfterPrefix)
        {
            
            int len = str.Length;
            if(len <= rangeAfterPrefix) {
                int offset = rangeAfterPrefix - len;
                for(int i =0; i < offset; i++) {
                    str += " ";
                }
            }
            return str;
            
        }


        #endregion //App Config Helper

    }
}

