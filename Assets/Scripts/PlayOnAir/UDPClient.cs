using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPClient : SingletonBehaviour<UDPClient>
{
    public UdpClient client;
    public UdpClient client_reset;

    string udp_ip = "127.0.0.1";
    int udp_port = 1004;
    int udp_indexPort = 1005;

    protected override void Init()
    {
        //throw new NotImplementedException();
        client = new UdpClient();
        client_reset = new UdpClient();
    }

    public void SendData(string message)
    {
        if (client == null)
            client = new UdpClient();

        byte[] data = Encoding.UTF8.GetBytes(message);
        client.Send(data, data.Length, udp_ip, udp_port);
    }

    public void SendReset(string message)
    {
        if (client_reset == null)
            client_reset = new UdpClient();

        byte[] data = Encoding.UTF8.GetBytes(message);
        client.Send(data, data.Length, udp_ip, udp_port);
    }

    // Example usage (you can call this method from your game logic)
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        string message = "Hello from client!";
    //        SendData(message);
    //    }
    //}
}