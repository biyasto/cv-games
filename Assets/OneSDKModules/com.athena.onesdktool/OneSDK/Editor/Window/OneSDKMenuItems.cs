
using UnityEditor;
using UnityEngine;

namespace OneSDK.Editor
{
    public class OneSDKMenuItems
    {
        [MenuItem("OneSDK/SDK Manager")]
    
        private static void ShowOneSDKToolManager()
        {
            OneSDKManagerWindow.ShowSDKManager();
        }
        
        [MenuItem("OneSDK/Documentation")]
        private static void Documentation()
        {
            Application.OpenURL("https://google.com");
        }

        [MenuItem("OneSDK/Contact Us")]
        private static void ContactUs()
        {
            Application.OpenURL("https://google.com/");
        }

        [MenuItem("OneSDK/About")]
        private static void About()
        {
            Application.OpenURL("https://google.com");
        }
    }
}
