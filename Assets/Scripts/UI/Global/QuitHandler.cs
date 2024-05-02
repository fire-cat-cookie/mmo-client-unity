using Assets.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitHandler : MonoBehaviour {

    private LoginHandler loginHandler;

    void Awake() {
        loginHandler = FindObjectOfType<LoginHandler>();
        Application.quitting += Quit;
    }

    void Quit() {
        if (loginHandler != null) {
            loginHandler.Logout();
        }
    }
}
