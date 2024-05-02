using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterMenu : MonoBehaviour {

    private LoginHandler loginHandler;
    private ScreenTransitionHandler screenTransitionHandler;
    
    public TMP_InputField idField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public Button backButton;
    public Button confirmButton;
    public MessagePopup messagePopup;

    private bool receivedResponse = false;

    void Awake() {
        loginHandler = FindObjectOfType<LoginHandler>();
        screenTransitionHandler = FindObjectOfType<ScreenTransitionHandler>();
    }
	
    void Start() {
        confirmButton.onClick.AddListener(Confirm);
        backButton.onClick.AddListener(Back);

        loginHandler.AccountCreated += OnAccountCreated;
        loginHandler.ErrorUsernameIsTaken += OnErrorUsernameTaken;

        idField.Select();
    }

    private void OnErrorUsernameTaken() {
        receivedResponse = true;
        messagePopup.ShowMessage("Username is already taken");
    }

    private void OnAccountCreated() {
        receivedResponse = true;
        screenTransitionHandler.ShowLoginMenu();
        messagePopup.ShowMessage("Account created.");
    }

    void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
            Confirm();
        }
	}

    private void Confirm() {
        if (idField.text.Length < 3 || idField.text.Length > 20) {
            messagePopup.ShowMessage("Username must be 3 to 20 characters long.");
            return;
        }
        if (passwordField.text.Length < 3 || passwordField.text.Length > 20) {
            messagePopup.ShowMessage("Password must be 3 to 20 characters long.");
            return;
        }
        if (!passwordField.text.Equals(confirmPasswordField.text)) {
            messagePopup.ShowMessage("Passwords do not match");
            return;
        }
        messagePopup.Hide();
        loginHandler.CreateAccount(idField.text, passwordField.text);
        Invoke("TimeoutCheck", 1f);
    }

    private void TimeoutCheck() {
        if (!receivedResponse) {
            messagePopup.ShowMessage("Server is not reachable");
        }
    }

    private void Back() {
        screenTransitionHandler.ShowLoginMenu();
    }
}
