using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUIService : MonoBehaviour{

    public List<UIElement> uiPrefabs;

    [Serializable]
    public class UIElement {
        public GameObject element;
        public Vector2 relativePosition;

        public UIElement(GameObject element, Vector2 relativePosition) {
            this.element = element;
            this.relativePosition = relativePosition;
        }
    }

    private PlayerService playerService;
    private CameraController cameraController;
    private new Camera camera;

    private Dictionary<Player, List<UIElement>> uiElements = new Dictionary<Player, List<UIElement>>();
    
	void Awake() {
        playerService = FindObjectOfType<PlayerService>();
        cameraController = FindObjectOfType<CameraController>();
        camera = cameraController.gameObject.GetComponent<Camera>();
        cameraController.CameraMoved += UpdateMainCharacter;
        playerService.PlayerAdded += AddPlayer;
        playerService.PlayerRemoved += RemovePlayer;
    }

    void Start() {
        foreach(Player player in playerService.GetAllPlayers()) {
            AddPlayer(player);
        }
    }

    void Update() {
        foreach(Player player in uiElements.Keys) {
            UpdateUIElements(player);
        }
    }

    private void UpdateUIElements(Player player) {
        foreach(UIElement ui in uiElements[player]) {
            Vector3 playerPosition = camera.WorldToScreenPoint(player.gameObject.transform.position);
            ui.element.transform.position = playerPosition;
            ui.element.transform.Translate(ui.relativePosition.x, ui.relativePosition.y, 0);
        }
    }

    private void UpdateMainCharacter() {
        if (!playerService.MainPlayerId.HasValue) {
            return;
        }
        Player player = playerService.FindPlayer(playerService.MainPlayerId.Value);
        if (uiElements.ContainsKey(player)) {
            UpdateUIElements(player);
        }
    }

    private void AddPlayer(Player player) {
        foreach(UIElement prefab in uiPrefabs) {
            GameObject instance = GameObject.Instantiate(prefab.element);
            instance.transform.SetParent(gameObject.transform, false);

            if (!uiElements.ContainsKey(player)) {
                uiElements[player] = new List<UIElement>();
            }
            uiElements[player].Add(new UIElement(instance, prefab.relativePosition));

            Healthbar healthBar = instance.GetComponent<Healthbar>();
            if (healthBar != null) {
                healthBar.Player = player;
            }

            SpeechBubble speechBubble = instance.GetComponent<SpeechBubble>();
            if (speechBubble != null) {
                speechBubble.Player = player;
            }
        }
    }

    private void RemovePlayer(Player player) {
        if (uiElements.ContainsKey(player)) {
            foreach(UIElement uiElement in uiElements[player]) {
                GameObject.Destroy(uiElement.element);
            }
            uiElements.Remove(player);
        }
    }

}
