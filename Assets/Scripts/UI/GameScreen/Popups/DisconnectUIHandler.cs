using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectUIHandler : MonoBehaviour {

    private LoginHandler loginHandler;

    public MessagePopup popup;

    void Awake() {
        loginHandler = FindObjectOfType<LoginHandler>();
        loginHandler.SelfDisconnect += OnDisconnected;
        if (popup == null) {
            throw new ArgumentNullException();
        }
    }

	void OnDisconnected() {
        popup.ShowMessage("Disconnected from server");
    }
}
