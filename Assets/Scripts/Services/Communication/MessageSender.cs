using mmo_shared;
using mmo_shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MessageSender : MonoBehaviour {
    private ServerConnector server;
    private EncryptionService encryption;
    private LoginHandler loginHandler;
    private Serializer serializer;

    void Awake() {
        encryption = FindObjectOfType<EncryptionService>();
        server = FindObjectOfType<ServerConnector>();
        loginHandler = FindObjectOfType<LoginHandler>();
        SharedServices shared = FindObjectOfType<SharedServices>();
        serializer = shared.serializer;
    }

    public void Send(Message msg) {
        byte[] packet = encryption.Encrypt(serializer.Serialize(msg), 0);
        server.Send(packet);
    }
}
