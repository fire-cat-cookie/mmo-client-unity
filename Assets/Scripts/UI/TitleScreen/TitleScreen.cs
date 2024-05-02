using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {
    public Button quitButton;

    void Start () {
        quitButton.onClick.AddListener(QuitPressed);
    }

    private void QuitPressed() {
        Application.Quit();
    }
}
