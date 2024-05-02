using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour{

    public Player Player { get; set; }
    [Header("Optional")]
    public TMP_Text valueDisplay;

    private Slider healthSlider;
    
	void Awake(){
        healthSlider = GetComponent<Slider>();
	}
	
    void Start(){
        
    }

    void Update() {
        healthSlider.value = Player.CharInfo.CurrentHealth / Player.CharInfo.MaxHealth;
        if (valueDisplay != null) {
            valueDisplay.text = $"{Mathf.Ceil(Player.CharInfo.CurrentHealth)} / {Mathf.Ceil(Player.CharInfo.MaxHealth)}";
        }
    }
	
}
