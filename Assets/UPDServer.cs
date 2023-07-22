using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.IO;

public class UPDServer : MonoBehaviour
{
    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    static IPAddress serverAddr =  IPAddress.Parse("127.0.0.1");
    private IPEndPoint endPoint = new IPEndPoint(serverAddr, 5059);
    static private string text = "hello from unity";
    private byte[] send_buffer = Encoding.ASCII.GetBytes(text );


    private void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            text = "b";
        }
        if (Input.GetKeyDown("n"))
        {
            text = "n";
        }
        if (Input.GetKeyDown("m"))
        {
            text="m";
        }
        SendData();
    }

    void SendData()
    {
        
        send_buffer =  Encoding.ASCII.GetBytes(text );
        sock.SendTo(send_buffer, endPoint);
        Debug.Log(text);
    }
}