using mmo_shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skillbar : MonoBehaviour {

    public Image[] cooldownCovers;

    private InputHandler inputHandler;
    private SkillInputService skillService;

    void Awake(){
        inputHandler = FindObjectOfType<InputHandler>();
        skillService = FindObjectOfType<SkillInputService>();
	}
	
    void Start(){
        
    }

    void Update() {
        for (int slot=0; slot<cooldownCovers.Length; slot++) {
            Skill skill = skillService.GetAssignedSkill(slot);
            cooldownCovers[slot].fillAmount = skillService.GetCurrentCooldown(skill) / skill.Cooldown;
        }
    }
	
}
