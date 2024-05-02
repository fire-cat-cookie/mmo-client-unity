using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using mmo_shared;

public class TargetInfoUI : MonoBehaviour{

    public GameObject window;
    public Healthbar healthbar;
    public TMP_Text displayName;
    public TMP_Text infoText;

    private InputHandler inputHandler;
    private PlayerService playerService;
    private LoginHandler loginHandler;

    private Player currentTarget;
    
	void Awake() {
        inputHandler = FindObjectOfType<InputHandler>();
        playerService = FindObjectOfType<PlayerService>();
        loginHandler = FindObjectOfType<LoginHandler>();
        inputHandler.InteractCommand += OnSelect;
        inputHandler.Select += OnSelect;
        inputHandler.Deselect += Hide;
        loginHandler.ForeignDisconnect += OnPlayerLeft;
    }
	
    void Start(){
        
    }

    private void OnSelect(GameObject target) {
        Player player = target.GetComponent<Player>();
        if (player) {
            Display(player);
        }
    }

    private void Display(Player player) {
        currentTarget = player;
        window.SetActive(true);
        healthbar.Player = player;
        string alignment = (player == playerService.GetMainPlayer()) ? "" : "Enemy : ";
        displayName.text = alignment + player.CharInfo.Name;
        infoText.text = $"Lv. {player.CharInfo.Level} {ClassData.Names[player.CharInfo.ClassId]}";
    }

    private void Hide() {
        currentTarget = null;
        window.SetActive(false);
    }

    private void OnPlayerLeft(uint playerId) {
        if (playerId == currentTarget.CharInfo.PlayerId) {
            Hide();
        }
    }
	
}
