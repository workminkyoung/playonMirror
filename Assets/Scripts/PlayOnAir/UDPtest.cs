using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDP : MonoBehaviour
{
    private UdpClient server;
    private int port = 8888;

    private void Start()
    {
        server = new UdpClient(port);
        server.BeginReceive(ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        byte[] data = server.EndReceive(ar, ref remoteEndPoint);
        string message = Encoding.UTF8.GetString(data);

        Debug.Log("Received message from " + remoteEndPoint.Address + ": " + message);

        // Handle the received message here (e.g., update the game state)

        server.BeginReceive(ReceiveCallback, null);
    }
}
