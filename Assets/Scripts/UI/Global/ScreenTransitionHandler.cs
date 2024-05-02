using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionHandler : MonoBehaviour {

    public GameObject loginScreen;
    public GameObject loginMenu;
    public GameObject registerMenu;
    public GameObject gameScreen;
    public GameObject charSelectScreen;

    private InputHandler inputHandler;

    void Awake() {
        inputHandler = FindObjectOfType<InputHandler>();
    }

    public void HideAll() {
        loginScreen.SetActive(false);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
        gameScreen.SetActive(false);
        charSelectScreen.SetActive(false);
    }

    public void ShowLoginScreen() {
        Show(loginScreen);
        inputHandler.keyboardContext = KeyboardContext.None;
    }

    public void ShowLoginMenu() {
        Show(loginScreen, loginMenu);
    }

    public void ShowRegisterMenu() {
        Show(loginScreen, registerMenu);
    }

    public void ShowGameScreen() {
        Show(gameScreen);
        inputHandler.keyboardContext = KeyboardContext.Game;
    }

    public void ShowCharSelectScreen() {
        Show(loginScreen, charSelectScreen);
    }

    private void Show(GameObject ui) {
        HideAll();
        ui.SetActive(true);
    }

    private void Show(GameObject ui, GameObject nestedUI) {
        HideAll();
        ui.SetActive(true);
        nestedUI.SetActive(true);
    }
}
