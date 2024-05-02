using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatService : MonoBehaviour{

    public delegate void MessageReceivedHandler(ServerChatMessage message);
    public event MessageReceivedHandler MessageReceived = delegate { };

    private MessageSender messageSender;
    private PacketPublisher packetPublisher;
    
	void Awake(){
        messageSender = FindObjectOfType<MessageSender>();
        packetPublisher = FindObjectOfType<PacketPublisher>();
        packetPublisher.Subscribe(typeof(ServerChatMessage), (Message msg) => { MessageReceived(msg as ServerChatMessage); });
    }

    public void Send(string text) {
        messageSender.Send(new ClientChatMessage(text));
    }
	
}
