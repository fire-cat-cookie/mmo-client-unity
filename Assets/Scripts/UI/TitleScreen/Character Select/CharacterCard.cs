using mmo_shared;
using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour {

    public TMP_Text charName;
    public TMP_Text levelAndClass;
    public TMP_Text location;
    public Image charImage;

	void Start () {
		
	}

    public void SetCharacter(CharSlotInfo charInfo) {
        charName.text = charInfo.Name;
        string className = "Unknown";
        if (charInfo.Class < ClassData.Names.Length) {
            className = ClassData.Names[charInfo.Class];
        }
        levelAndClass.text = $"Level {charInfo.Level} {className}";
        string zoneName = "Unknown";
        if (charInfo.Location < ZoneData.Names.Length) {
            zoneName = ZoneData.Names[charInfo.Location];
        }
        location.text = "Location: " + zoneName;
    }
}
