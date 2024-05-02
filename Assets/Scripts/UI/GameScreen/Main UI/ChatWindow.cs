using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour{

    public TMP_Text chatWindow;
    public TMP_InputField chatInputField;

    private InputHandler inputHandler;
    private ChatService chatService;
    private PlayerService playerService;

    private List<string> chatHistory = new List<string>();
    private readonly int maxDisplayedMessages = 50;

    void Awake(){
        inputHandler = FindObjectOfType<InputHandler>();
        chatService = FindObjectOfType<ChatService>();
        playerService = FindObjectOfType<PlayerService>();

        chatInputField.onFocusSelectAll = false;
        chatService.MessageReceived += OnMessageReceived;
    }
	
    void Start(){
        
    }

    void Update() {
        if (inputHandler.keyboardContext == KeyboardContext.Game) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                inputHandler.keyboardContext = KeyboardContext.Chat;
                ToggleChatBar(true);
            }
        } else if (inputHandler.keyboardContext == KeyboardContext.Chat) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(chatInputField.gameObject);
            if (Input.GetKeyDown(KeyCode.Return)) {
                SendMessage();
                ToggleChatBar(false);
                inputHandler.keyboardContext = KeyboardContext.Game;
            } else if (Input.GetKeyDown(KeyCode.Escape)) {
                chatInputField.text = "";
                ToggleChatBar(false);
                inputHandler.keyboardContext = KeyboardContext.Game;
            }
        }
    }

    private void OnMessageReceived(ServerChatMessage message) {
        Player player = playerService.FindPlayer(message.SenderId);
        if (player == null) {
            return;
        }

        string displayText = $"{player.CharInfo.Name} : {message.Text}\n";
        chatHistory.Add(displayText);
        if (chatHistory.Count > maxDisplayedMessages) {
            chatHistory.RemoveAt(0);
        }
        chatWindow.text = "";
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string chatty in chatHistory) {
            stringBuilder.Append(chatty);
        }
        chatWindow.text = stringBuilder.ToString();
    }

    private void ToggleChatBar(bool toggle) {
        chatInputField.gameObject.SetActive(toggle);
    }

    private void SendMessage() {
        if (!chatInputField.text.Equals("")) {
            chatService.Send(chatInputField.text);
            chatInputField.text = "";
        }
    }
	
}
