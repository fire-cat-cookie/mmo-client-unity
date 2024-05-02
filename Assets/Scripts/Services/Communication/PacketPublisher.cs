using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Net;
using mmo_shared.Messages;
using System;
using mmo_shared;
using System.Collections.Concurrent;

/// <summary>
/// Provides a framework for publishing incoming client messages for other services to subscribe to.
/// </summary>
public class PacketPublisher : MonoBehaviour {
    private ServerConnector server;
    private SharedServices sharedServices;

    public delegate void HandleMessage(Message message);

    private ConcurrentQueue<Message> messageBuffer = new ConcurrentQueue<Message>();
    private Dictionary<Type, List<HandleMessage>> packetHandlers = new Dictionary<Type, List<HandleMessage>>();

    void Awake() {
        server = FindObjectOfType<ServerConnector>();
        sharedServices = FindObjectOfType<SharedServices>();
    }

    void Start() {
        server.PacketReceived += OnPacketReceived;
    }

    void Update() {
        while (!messageBuffer.IsEmpty) {
            messageBuffer.TryDequeue(out Message message);
            foreach (HandleMessage handler in packetHandlers[message.GetType()]) {
                handler.Invoke(message);
            }
        }
    }

    private void OnPacketReceived(byte[] packet) {
        Type packetType = sharedServices.serializer.GetType(packet, 0, false);
        if (packetType == null) {
            return;
        }
        Message message = sharedServices.serializer.Deserialize(packet, false);
        if (message == null) {
            return;
        }
        if (packetHandlers.ContainsKey(message.GetType())) {
            messageBuffer.Enqueue(message);
        }
    }

    public void Subscribe(Type packetType, HandleMessage handler) {
        if (!packetHandlers.ContainsKey(packetType)) {
            packetHandlers[packetType] = new List<HandleMessage>();
        }
        packetHandlers[packetType].Add(handler);
    }

}
