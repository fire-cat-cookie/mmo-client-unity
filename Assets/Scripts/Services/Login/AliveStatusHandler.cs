using Assets.Scripts.Services;
using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AliveStatusHandler : MonoBehaviour {
    private MessageSender messageSender;
    private LoginHandler loginHandler;
    private PacketPublisher packetPublisher;

    void Awake() {
        messageSender = FindObjectOfType<MessageSender>();
        loginHandler = FindObjectOfType<LoginHandler>();
        packetPublisher = FindObjectOfType<PacketPublisher>();

        loginHandler.LoginSuccess += StayingAlive;
    }

    private void StayingAlive() {
        if (loginHandler.IsLoggedIn) {
            messageSender.Send(new Alive());
            Invoke("StayingAlive", 1f);
        }
    }
}
