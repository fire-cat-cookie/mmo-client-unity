using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnWindow : MonoBehaviour {

    public GameObject respawnDialog;
    public Button respawnButton;

    private PlayerService playerSerivce;
    private MessageSender messageSender;
    private HealthService healthService;

    void Awake() {
        playerSerivce = FindObjectOfType<PlayerService>();
        messageSender = FindObjectOfType<MessageSender>();
        healthService = FindObjectOfType<HealthService>();
        healthService.MainPlayerDied += OnDeath;
        respawnButton.onClick.AddListener(Respawn);
    }

    void Start() {

    }

    private void OnDeath() {
        respawnDialog.SetActive(true);
    }

    private void Respawn() {
        messageSender.Send(new ClientRespawn());
        respawnDialog.SetActive(false);
    }

}
