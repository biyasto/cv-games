using System;
using System.Collections;
using System.Collections.Generic;
namespace OneSDK.Editor
{
    public enum PluginType
    {
        UnityPackage = 0,
        Tarball = 1,
        Git = 2
    }
    public enum PluginState
    {
        NotInstalled,
        Installed,
        NeedUpdate
    }
    [Serializable]
    public class PluginInfo
    {
        public string Id;
        public string Name;
        public PluginType PluginType;
        public string Version;
        public string DownloadUrl;
        public string[] ContentManifest;
        public string[] Dependencies;

        [NonSerialized]
        public PluginState PluginState;
        [NonSerialized]
        public string LocalVersion;

        public void UpdatePluginInfo(PluginInfo other)
        {
            Id = other.Id;
            Name = other.Name;
            PluginType = other.PluginType;
            Version = other.Version;
            DownloadUrl = other.DownloadUrl;
            ContentManifest = other.ContentManifest;
            Dependencies = other.Dependencies;
            PluginState = other.PluginState;
            LocalVersion = other.LocalVersion;
        }

    }

    [Serializable]
    public class PluginManifest
    {
        public List<PluginInfo> Plugins;
    }

    public struct ConfigValue
    {
        public bool BooleanValue { get { return bool.Parse(StringValue); } }
        public double DoubleValue { get { return double.Parse(StringValue, System.Globalization.CultureInfo.GetCultureInfo("en-US")); } }
        public long LongValue { get { return long.Parse(StringValue); } }
        public int IntValue { get { return int.Parse(StringValue); } }
        public string StringValue { get; private set; }

        public ConfigValue(string rawValue)
        {
            StringValue = rawValue;
        }
    }

}
