using Assets.Scripts.Services;
using mmo_shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LoginHandler : MonoBehaviour {

    public delegate void LoginEventHandler();
    public delegate void CharInfoReceivedHandler(CharSelectInfo charInfo);
    public delegate void ForeignDisconnectHandler(uint playerId);

    public event LoginEventHandler LoginSuccess = delegate { };
    public event LoginEventHandler LoginTimeout = delegate { };
    public event LoginEventHandler ErrorAlreadyLoggedIn = delegate { };
    public event LoginEventHandler IncorrectLogin = delegate { };
    public event LoginEventHandler SelfDisconnect = delegate { };
    public event LoginEventHandler ErrorUsernameIsTaken = delegate { };
    public event LoginEventHandler AccountCreated = delegate { };
    public event CharInfoReceivedHandler CharInfoReceived = delegate { };
    public event ForeignDisconnectHandler ForeignDisconnect = delegate { };

    public bool IsLoggedIn { get; private set; }


    private MessageSender messageSender;
    private PacketPublisher packetPublisher;
    private PlayerService playerService;
    private Player mainPlayer;

    private bool receivedLoginResponse = false;
    private byte[] sessionId;

    void Awake() {
        messageSender = FindObjectOfType<MessageSender>();
        packetPublisher = FindObjectOfType<PacketPublisher>();
        playerService = FindObjectOfType<PlayerService>();
        playerService.MainPlayerFound += (value) => { mainPlayer = value; };
        packetPublisher.Subscribe(typeof(SessionStart), ProcessSessionStart);
        packetPublisher.Subscribe(typeof(LoginResponse), ProcessLoginResponse);
        packetPublisher.Subscribe(typeof(CharSelectInfo), ProcessCharSelectInfo);
        packetPublisher.Subscribe(typeof(RegistrationResponse), ProcessRegistrationResponse);
        packetPublisher.Subscribe(typeof(ServerDisconnect), ProcessServerDisconnect);
    }

    void FixedUpdate() {
        if (sessionId == null) {
            messageSender.Send(new SessionIdRequest());
        }
    }

    public void CreateAccount(string username, string password) {
        if (sessionId == null) {
            return;
        }
        messageSender.Send(new CreateAccount() { Username = username, Password = password, SessionId = sessionId });
    }

    public void Logout() {
        ProcessLogout();
        messageSender.Send(new ClientDisconnect());
    }

    public void ProcessLogout() {
        IsLoggedIn = false;
        sessionId = null;
        SelfDisconnect();
    }

    private void ProcessSessionStart(Message message) {
        SessionStart session = message as SessionStart;
        sessionId = session.SessionId;
    }

    private void ProcessLoginResponse(Message message) {
        LoginResponse response = message as LoginResponse;
        receivedLoginResponse = true;
        if ((LoginResponse.Types)response.Type == LoginResponse.Types.IncorrectUsernameOrPassword) {
            IncorrectLogin();
        } else if ((LoginResponse.Types)response.Type == LoginResponse.Types.AlreadyLoggedIn) {
            ErrorAlreadyLoggedIn();
        }
    }

    private void ProcessCharSelectInfo(Message message) {
        CharSelectInfo info = message as CharSelectInfo;
        IsLoggedIn = true;
        LoginSuccess();
        CharInfoReceived(info);
    }

    private void ProcessRegistrationResponse(Message message) {
        RegistrationResponse response = message as RegistrationResponse;
        if ((RegistrationResponse.Types)response.ResponseType == RegistrationResponse.Types.Success) {
            AccountCreated();
        } else if ((RegistrationResponse.Types)response.ResponseType == RegistrationResponse.Types.UsernameTaken) {
            ErrorUsernameIsTaken();
        }
    }

    private void ProcessServerDisconnect(Message message) {
        ServerDisconnect disconnect = message as ServerDisconnect;
        if (mainPlayer != null && disconnect.PlayerId != mainPlayer.CharInfo.PlayerId) {
            ForeignDisconnect(disconnect.PlayerId);
        }
    }

    public void Login(string username, string password) {
        Invoke(nameof(TimeoutCheck), 1f);
        if (sessionId == null) {
            return;
        }
        receivedLoginResponse = false;
        messageSender.Send(new Login() { Username = username, Password = password, SessionId = sessionId });
    }

    private void TimeoutCheck() {
        if (!receivedLoginResponse) {
            sessionId = null;
            LoginTimeout();
        }
    }
}
