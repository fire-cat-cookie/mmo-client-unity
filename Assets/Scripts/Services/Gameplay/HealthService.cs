using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthService : MonoBehaviour {
    public delegate void EventHandler();
    public event EventHandler MainPlayerDied = delegate { };

    private PacketPublisher packetPublisher;
    private PlayerService playerService;
    
	void Awake(){
        packetPublisher = FindObjectOfType<PacketPublisher>();
        playerService = FindObjectOfType<PlayerService>();
        packetPublisher.Subscribe(typeof(HealthChange), ProcessHealthChange);
        packetPublisher.Subscribe(typeof(UnitRevive), ProcessRevive);
        packetPublisher.Subscribe(typeof(UnitDie), ProcessUnitDie);
    }

    private void ProcessHealthChange(Message message) {
        HealthChange healthChange = message as HealthChange;
        Player player = playerService.FindPlayer(healthChange.PlayerId);
        if (player != null) {
            player.CharInfo.CurrentHealth = healthChange.NewHealth;
        }
    }

    private void ProcessRevive(Message message) {
        UnitRevive revive = message as UnitRevive;
        Player player = playerService.FindPlayer(revive.UnitId);
        if (player != null) {
            player.CharInfo.Alive = true;
            player.Revive();

            Player mainPlayer = playerService.GetMainPlayer();
            if (player != mainPlayer) {
                player.gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
        }
    }

    private void ProcessUnitDie(Message message) {
        UnitDie die = message as UnitDie;
        Player player = playerService.FindPlayer(die.PlayerId);
        if (player != null) {
            player.CharInfo.Alive = false;
            player.gameObject.layer = LayerMask.NameToLayer("Default");
            player.Die();

            Player mainPlayer = playerService.GetMainPlayer();
            if (player == mainPlayer) {
                MainPlayerDied();
            }
        }
    }

}
