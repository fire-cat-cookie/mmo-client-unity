using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour {

    public Button button;
    public Image charImage;
    public Image highlight;

    public delegate void ClickHandler();
    public event ClickHandler Clicked = delegate { };

    void Awake() {
        button.onClick.AddListener(() => Clicked());
    }

	void Start () {
		
	}

    public void SetCharacter(CharSlotInfo info) {
        charImage.gameObject.SetActive(info != null);
    }

    public void Deselect() {
        highlight.gameObject.SetActive(false);
    }

    public void Select() {
        highlight.gameObject.SetActive(true);
    }
}
