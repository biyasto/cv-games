
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

    public class UDPReceive : MonoBehaviour
    {
        private Thread receiveThread;

        private UdpClient client;
        public  int port = 5099;

        public bool startReceiving = true;
        public bool isReceived = false;
        public bool printToConsole = false;

        public string data="[0.5, 0.5, 0.5, 0.4, 0.5, 0.6]";
        // Start is called before the first frame update
        void Start()
        {
          
          
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        

        private void ReceiveData()
        {
            client = new UdpClient(port);
            while (startReceiving)
            {
                try
                {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] dataByte = client.Receive(ref anyIP);
                    var tmpData = Encoding.UTF8.GetString(dataByte);
                    if (tmpData!= "")
                    {
                        data = tmpData;
                    }
                    else
                    {
                        Debug.Log("Something went wrong chief -> try restart camera");
                    }

                    if (!isReceived)
                    {
                        isReceived = true;
                        Debug.Log("udp ready");
                        
                    }
                }
                catch (Exception err)
                {
                    print("no data");
                    print(err.ToString());
                }
            }
        }
        
      
    }