using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerConnector : MonoBehaviour {
    
    private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private readonly IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2057);
    private readonly byte[] receiveBuffer = new byte[UInt16.MaxValue];

    public delegate void ProcessPacket(byte[] packet);
    public event ProcessPacket PacketReceived = delegate { };

    void Awake() {
        socket.SendTimeout = 1000;
        socket.Connect(serverAddress);
    }

    void Start () {
        Thread receiverThread = new Thread(Receive);
        receiverThread.Start();
    }

    private void Receive() {
        EndPoint remote = serverAddress;
        while (true) {
            try {
                if (socket.Available == 0) {
                    continue;
                }
                int packetSize = socket.ReceiveFrom(receiveBuffer, ref remote);
                byte[] packet = new byte[packetSize];
                Array.Copy(receiveBuffer, packet, packetSize);

                PacketReceived(packet);
            } catch (SocketException e) {
                switch (e.SocketErrorCode) {
                    case SocketError.ConnectionReset:
                        continue;
                }
                Debug.Log(e.ErrorCode);
            }
        }
    }

    public void Send(byte[] data) {
        socket.Send(data);
    }
}
