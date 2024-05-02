using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using mmo_shared.Messages;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

    public Player Player { get; set; }

    private TMP_Text textComponent;
    private Image background;
    private ChatService chatService;
    private PlayerService playerService;

    private readonly float stayingTime = 5f;
    private float screenTime;

    void Awake() {
        textComponent = GetComponentInChildren<TMP_Text>();
        background = GetComponent<Image>();
        chatService = FindObjectOfType<ChatService>();
        playerService = FindObjectOfType<PlayerService>();
        chatService.MessageReceived += ShowMessage;
    }
	
    void Start() {
        SetVisible(false);
    }

    void Update() {
        screenTime += Time.deltaTime;
        if (screenTime >= stayingTime) {
            SetVisible(false);
        }
    }

    private void SetVisible(bool visible) {
        textComponent.gameObject.SetActive(visible);
        background.enabled = visible;
    }

    private void ShowMessage(ServerChatMessage message) {
        Player player = playerService.FindPlayer(message.SenderId);
        if (player == null || this.Player.CharInfo.PlayerId != player.CharInfo.PlayerId) {
            return;
        }
        SetVisible(true);
        screenTime = 0;
        string displayText = $"{player.CharInfo.Name} : {message.Text}\n";
        textComponent.text = displayText;
    }
	
}
