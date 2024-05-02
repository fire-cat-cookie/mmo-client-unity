using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerService : MonoBehaviour {

    public delegate void PlayerEventHandler(Player player);
    public event PlayerEventHandler MainPlayerFound = delegate { };
    public event PlayerEventHandler PlayerAdded = delegate { };
    public event PlayerEventHandler PlayerRemoved = delegate { };

    public GameObject playerPrefab;
    public uint? MainPlayerId { get; set; }

    private PacketPublisher packetPublisher;
    private LoginHandler loginHandler;

    private Dictionary<uint, Player> players = new Dictionary<uint, Player>(); //maps playerIds to players.
    private Dictionary<Player, uint> playerIds = new Dictionary<Player, uint>(); //maps players to playerIds.

    void Awake(){
        packetPublisher = FindObjectOfType<PacketPublisher>();
        loginHandler = FindObjectOfType<LoginHandler>();
        loginHandler.ForeignDisconnect += OnPlayerLeft;
        packetPublisher.Subscribe(typeof(CharInfo), ProcessCharInfo);
        packetPublisher.Subscribe(typeof(PositionUpdate), ProcessPositionUpdate);
    }

    public List<Player> GetAllPlayers() {
        return new List<Player>(players.Values);
    }

    public void AddPlayer(Player player, uint playerId) {
        players[playerId] = player;
        playerIds[player] = playerId;
    }

    public void RemovePlayer(Player player) {
        PlayerRemoved(player);
        if (playerIds.ContainsKey(player)) {
            playerIds.Remove(player);
            players.Remove(player.CharInfo.PlayerId);
            GameObject.Destroy(player.gameObject);
        }
    }

    public Player FindPlayer(uint playerId) {
        if (!players.ContainsKey(playerId)) {
            return null;
        }
        return players[playerId];
    }

    public Player GetMainPlayer() {
        if (MainPlayerId.HasValue && players.ContainsKey(MainPlayerId.Value)) {
            return players[MainPlayerId.Value];
        }
        return null;
    }

    public bool CanMove(Player player) {
        return player.CharInfo.Alive;
    }

    public bool CanAttack(Player attackingPlayer) {
        return attackingPlayer.CharInfo.Alive;
    }

    private void ProcessCharInfo(Message message) {
        CharInfo charSpotted = message as CharInfo;
        if (!MainPlayerId.HasValue) {
            return;
        }
        if (!players.ContainsKey(charSpotted.PlayerId)) {
            CreatePlayer(charSpotted);
        }
    }

    private void ProcessPositionUpdate(Message message) {
        PositionUpdate update = message as PositionUpdate;
        Player player;
        if (players.ContainsKey(update.PlayerId)) {
            player = players[update.PlayerId];
            ApplyPositionUpdate(update);
        }
    }

    private void ApplyPositionUpdate(PositionUpdate update) {
        Player p = players[update.PlayerId];
        var yPos = Terrain.activeTerrain.SampleHeight(new Vector3(update.Position.X, 0, update.Position.Y));
        var yDestination = Terrain.activeTerrain.SampleHeight(new Vector3(update.Destination.X, 0, update.Destination.Y));
        p.transform.position = new Vector3(update.Position.X, yPos, update.Position.Y);
        p.SetDestination(new Vector3(update.Destination.X, yDestination, update.Destination.Y));
        p.CharInfo.Position = update.Position;
        p.CharInfo.Velocity = update.Velocity;
        p.CharInfo.Destination = update.Destination;
    }

    private Player CreatePlayer(CharInfo info) {
        Vector3 position = new Vector3(info.Position.X, Terrain.activeTerrain.SampleHeight(new Vector3(info.Position.X, 0, info.Position.Y)), info.Position.Y);
        GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);

        Player player = playerObject.GetComponent<Player>();
        AddPlayer(player, info.PlayerId);
        player.CharInfo = info;
        player.SetDestination(new Vector3 (info.Destination.X, 0, info.Destination.Y));

        PlayerAdded(player);

        if (MainPlayerId.HasValue && info.PlayerId == MainPlayerId.Value) {
            MainPlayerFound(player);
        }

        return player;
    }

    private void OnPlayerLeft(uint playerId) {
        if (!players.ContainsKey(playerId)) {
            return;
        }
        RemovePlayer(players[playerId]);
    }
	
}
