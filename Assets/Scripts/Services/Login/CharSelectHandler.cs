using Assets.Scripts.Services;
using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelectHandler : MonoBehaviour {

    private MessageSender messageSender;
    private PacketPublisher packetPublisher;
    private PlayerService playerService;

    public delegate void EventHandler();
    public event EventHandler TimedOut = delegate { };
    public delegate void CharacterCreatedHandler(CharSlotInfo newCharacter);
    public event CharacterCreatedHandler CharacterCreated = delegate { };


    private bool receivedResponse = false;

    void Awake() {
        messageSender = FindObjectOfType<MessageSender>();
        packetPublisher = FindObjectOfType<PacketPublisher>();
        playerService = FindObjectOfType<PlayerService>();
        packetPublisher.Subscribe(typeof(LoginCharacterResponse), ProcessCharacterLogin);
        packetPublisher.Subscribe(typeof(CreateCharacterResponse), ProcessCharacterCreation);
    }

    public void LoginCharacter(int slot) {
        messageSender.Send(new LoginCharacter() { Slot = (byte)slot });
        receivedResponse = false;
        Invoke(nameof(TimeoutCheck), 1f);
    }

    public void CreateCharacter(int slot, string name) {
        messageSender.Send(new CreateCharacter() { Slot = (byte)slot, Name = name });
        receivedResponse = false;
        Invoke(nameof(TimeoutCheck), 1f);
    }

    private void ProcessCharacterLogin(Message message) {
        LoginCharacterResponse response = message as LoginCharacterResponse;
        receivedResponse = true;
        playerService.MainPlayerId = response.PlayerId;
        messageSender.Send(new GetSurroundings());
    }

    private void ProcessCharacterCreation(Message message) {
        CreateCharacterResponse response = message as CreateCharacterResponse;
        receivedResponse = true;
        CharacterCreated(response.CharInfo);
    }

    private void TimeoutCheck() {
        if (!receivedResponse) {
            TimedOut();
        }
    }
	
}
