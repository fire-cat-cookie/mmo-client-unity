using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour{

    private InputHandler inputHandler;
    private MessageSender messageSender;
    private PlayerService playerService;
    private PacketPublisher packetPublisher;

    private GameObject lastInteractionTarget { get; set; }

    void Awake(){
        inputHandler = FindObjectOfType<InputHandler>();
        messageSender = FindObjectOfType<MessageSender>();
        playerService = FindObjectOfType<PlayerService>();
        packetPublisher = FindObjectOfType<PacketPublisher>();
        inputHandler.InteractCommand += OnInteract;
        inputHandler.TerrainMoveCommand +=(Vector3 value) => { playerService.GetMainPlayer().InterruptAttack = true; };
        packetPublisher.Subscribe(typeof(ServerAutoAttack), ProcessAutoAttack);
        packetPublisher.Subscribe(typeof(InterruptAttack), ProcessInterrupt);
    }

    void Update() {
        Player mainPlayer = playerService.GetMainPlayer();
        if (mainPlayer != null && mainPlayer.CurrentAttackTarget != null) {
            Player target = playerService.FindPlayer(mainPlayer.CurrentAttackTarget.CharInfo.PlayerId);
            if (target == null || !target.CharInfo.Alive) {
                mainPlayer.InterruptAttack = true;
            }
        }
    }

    private void ProcessAutoAttack(Message message) {
        ServerAutoAttack attack = message as ServerAutoAttack;
        Player source = playerService.FindPlayer(attack.SourcePlayerId);
        Player target = playerService.FindPlayer(attack.TargetPlayerId);
        if (source && target) {
            source.QueueAttack(target);
        }
    }

    private void ProcessInterrupt(Message message) {
        InterruptAttack interrupt = message as InterruptAttack;
        Player player = playerService.FindPlayer(interrupt.UnitId);
        if (player) {
            player.InterruptAttack = true;
        }
    }

    private void OnInteract(GameObject target) {
        if (target != lastInteractionTarget) {
            playerService.GetMainPlayer().InterruptAttack = true;
        }
        lastInteractionTarget = target;

        Player targetPlayer = target.GetComponent<Player>();
        Player mainPlayer = playerService.GetMainPlayer();
        if (mainPlayer != null && targetPlayer != null && targetPlayer != mainPlayer && targetPlayer.CharInfo.Alive) {
            if (!playerService.CanAttack(playerService.GetMainPlayer())) {
                return;
            }
            if (!mainPlayer.InAttackAnimation()) {
                messageSender.Send(new AutoAttack(targetPlayer.CharInfo.PlayerId));
                mainPlayer.QueueAttack(targetPlayer);
            }
        }
    }
	
}
