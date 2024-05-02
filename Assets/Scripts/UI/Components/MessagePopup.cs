using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup : MonoBehaviour {

    public TMP_Text text;
    public Button button;

    public void Awake() {
        button.onClick.AddListener(Hide);
        Hide();
    }

    public void ShowMessage(string message) {
        text.text = message;
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
