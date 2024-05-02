using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using mmo_shared.Messages;

public class LoginMenu : MonoBehaviour {
    private LoginHandler loginHandler;
    private ScreenTransitionHandler screenTransitionHandler;

    public TMP_InputField idField;
    public TMP_InputField passwordField;
    public Button loginButton;
    public Button signUpButton;
    public MessagePopup messagePopup;

    private string username = "";
    private string password = "";

    void Awake() {
        loginHandler = FindObjectOfType<LoginHandler>();
        screenTransitionHandler = FindObjectOfType<ScreenTransitionHandler>();
    }
    
    void Start() {
        loginHandler.LoginSuccess += OnLoginSuccess;
        loginHandler.IncorrectLogin += OnIncorrectLogin;
        loginHandler.ErrorAlreadyLoggedIn += OnErrorAlreadyLoggedIn;
        loginHandler.LoginTimeout += OnLoginTimeout;

        SetupInputFields();
        SetupButtons();

        idField.Select();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            EnterPressed();
        }
    }

    private void SetupInputFields() {
        idField.onEndEdit.AddListener(IdChanged);
        passwordField.onEndEdit.AddListener(PasswordChanged);
    }

    private void SetupButtons() {
        loginButton.onClick.AddListener(LoginPressed);
        signUpButton.onClick.AddListener(SignUpPressed);
    }

    private void EnterPressed() {
        loginButton.onClick.Invoke();
    }

    private void IdChanged(string input) {
        username = input;
    }

    private void PasswordChanged(string input) {
        password = input;
    }

    private void LoginPressed() {
        messagePopup.Hide();
        loginHandler.Login(username, password);
    }

    private void SignUpPressed() {
        screenTransitionHandler.ShowRegisterMenu();
    }

    private void OnLoginTimeout() {
        messagePopup.ShowMessage("Server is not reachable");
    }

    private void QuitPressed() {
        Application.Quit();
    }

    private void OnLoginSuccess() {
        if (!gameObject.activeInHierarchy) {
            return;
        }
        screenTransitionHandler.ShowCharSelectScreen();
    }

    private void OnIncorrectLogin() {
        if (!gameObject.activeInHierarchy) {
            return;
        }
        messagePopup.ShowMessage("Incorrect username or password");
    }

    private void OnErrorAlreadyLoggedIn() {
        if (!gameObject.activeInHierarchy) {
            return;
        }
        messagePopup.ShowMessage("User is already logged in");
    }
}
