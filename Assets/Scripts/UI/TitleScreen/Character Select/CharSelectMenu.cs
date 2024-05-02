using mmo_shared.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectMenu : MonoBehaviour {
    private LoginHandler loginHandler;
    private CharSelectHandler charSelectHandler;
    private ScreenTransitionHandler screenTransitionHandler;
    private PlayerService playerService;

    public Button backButton;
    public Button playButton;
    public MessagePopup messagePopup;

    public GameObject createMenu;
    public Button createButton;
    public TMP_InputField characterNameField;
    private string characterNameInput;

    public List<CharSlot> slots;
    public CharacterCard characterCard;

    private int maxNumberOfSlots = 3;
    private CharSlotInfo[] Chars { get; set; }
    private int selectedSlot = 0;


    void Awake() {
        charSelectHandler = FindObjectOfType<CharSelectHandler>();
        loginHandler = FindObjectOfType<LoginHandler>();
        screenTransitionHandler = FindObjectOfType<ScreenTransitionHandler>();
        playerService = FindObjectOfType<PlayerService>();

        Chars = new CharSlotInfo[maxNumberOfSlots];
        characterNameField.onEndEdit.AddListener((string input) => characterNameInput = input);
        loginHandler.CharInfoReceived += DisplayCharInfo;
    }

	void Start () {
        playerService.MainPlayerFound += OnCharacterLogin;
        charSelectHandler.CharacterCreated += OnCharacterCreated;
        charSelectHandler.TimedOut += OnTimeout;

        backButton.onClick.AddListener(Back);
        playButton.onClick.AddListener(Play);
        createButton.onClick.AddListener(CreateCharacter);
        for (int i=0; i< slots.Count; i++) {
            int j = i;
            slots[i].Clicked += () => SelectSlot(j);
        }
        SelectSlot(selectedSlot);
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.LeftArrow)){
            AdvanceSlot(-1);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow)){
            AdvanceSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (Chars[selectedSlot] != null) {
                Play();
            } else {
                CreateCharacter();
            }
        }
    }

    private void Back() {
        screenTransitionHandler.ShowLoginMenu();
        loginHandler.Logout();
    }

    private void AdvanceSlot(int change) {
        int select = Mathf.Clamp(selectedSlot + change, 0, maxNumberOfSlots-1);
        SelectSlot(select);
    }

    private void OnCharacterLogin(Player player) {
        screenTransitionHandler.ShowGameScreen();
    }

    private void OnCharacterCreated(CharSlotInfo newCharacter) {
        Chars[newCharacter.SlotId] = newCharacter;
        slots[newCharacter.SlotId].SetCharacter(newCharacter);
        SelectSlot(selectedSlot); //re-select to update information
    }

    private void Play() {
        charSelectHandler.LoginCharacter(selectedSlot);
    }
	
    private void CreateCharacter() {
        if (characterNameInput.Length < 3 || characterNameInput.Length > 20) {
            messagePopup.ShowMessage("Character name must be 3 to 20 characters long");
        } else {
            charSelectHandler.CreateCharacter(selectedSlot, characterNameInput);
        }
    }

    private void OnTimeout() {
        messagePopup.ShowMessage("Server is not reachable");
    }

    private void SelectSlot(int id) {
        selectedSlot = id;
        foreach (CharSlot slot in slots) {
            slot.Deselect();
        }
        slots[selectedSlot].Select();
        if (Chars[id] != null) {
            ShowCharacterCard(true);
            characterCard.SetCharacter(Chars[id]);
        } else {
            ShowCharacterCard(false);
        }
    }

    private void ShowCharacterCard(bool visible) {
        characterCard.gameObject.SetActive(visible);
        createMenu.gameObject.SetActive(!visible);
    }

    private void DisplayCharInfo(CharSelectInfo info) {
        Chars = new CharSlotInfo[maxNumberOfSlots];
        foreach (CharSlotInfo c in info.Chars) {
            if (c.SlotId < maxNumberOfSlots) {
                Chars[c.SlotId] = c;
                slots[c.SlotId].SetCharacter(c);
            }
        }
    }
}
