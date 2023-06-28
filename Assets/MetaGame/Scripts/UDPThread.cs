using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Debug = UnityEngine.Debug;

public class UDPThread : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string path = "/StreamingAssets/face.bat";
 
    private Thread receiveThread;
     
        void Start()
        {
            KillProcesses();
            
            ProcessStartInfo info = new ProcessStartInfo(Application.dataPath + path);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process proc = Process.Start(info);
            
        }
    

    private void KillProcesses()
    {
        foreach (var process in Process.GetProcessesByName("cmd"))
        {
            process.Kill();
        }
        foreach (var process in Process.GetProcessesByName("python"))
        {
            process.Kill();
        }
    }
    private void OnDestroy()
    {
        KillProcesses();
    }
}
