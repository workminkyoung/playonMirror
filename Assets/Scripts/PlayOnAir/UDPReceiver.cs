using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPReceiver : SingletonBehaviour<UDPReceiver>
{
    public Queue<string> Received = new Queue<string>();
    private UdpClient server;
    private bool isReceive = false;

    string udp_ip = "127.0.0.1";
    int udp_port = 1004;
    int udp_indexPort = 1005;

    protected override void Init()
    {
    }

    public void StartReceiveCall()
    {
        if (server == null)
            server = new UdpClient(udp_indexPort);

        isReceive = true;
        server.BeginReceive(ReceiveCallback, null);
    }

    public void StopReceiveCall()
    {
        isReceive = false;
        if(server != null)
        {
            CustomLogger.Log("Server Closed");
            server.Close();
            server = null;
        }
    }

    public string ReturnQueue()
    {
        string dequeue = Received.Dequeue();
        Received.Clear();
        return dequeue;
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, udp_port);
        byte[] data = server.EndReceive(ar, ref remoteEndPoint);
        string message = Encoding.UTF8.GetString(data);

        //CustomLogger.Log("Received message from " + remoteEndPoint.Address + ": " + message);
        Received.Enqueue(message);
        // Handle the received message here (e.g., update the game state)

        if (isReceive)
            server.BeginReceive(ReceiveCallback, null);
    }

    private void OnApplicationQuit()
    {
        StopReceiveCall();
    }

}
