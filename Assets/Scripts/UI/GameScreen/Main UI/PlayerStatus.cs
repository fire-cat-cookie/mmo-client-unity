using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

    public Healthbar healthbar;

    private PlayerService playerService;
    
	void Awake(){
        playerService = FindObjectOfType<PlayerService>();
    }
	
    void Start(){
        healthbar.Player = playerService.GetMainPlayer();
    }
	
}
