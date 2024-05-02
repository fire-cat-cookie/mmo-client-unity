using mmo_shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkHandler : MonoBehaviour {

    public GameObject blinkParticlesPrefab;

    private SkillUseService skillUseService;
    private PlayerService playerService;
    private ParticleService particleService;
    
	void Awake(){
        skillUseService = FindObjectOfType<SkillUseService>();
        playerService = FindObjectOfType<PlayerService>();
        particleService = FindObjectOfType<ParticleService>();
        skillUseService.Subscribe("Blink", Blink);
	}
	
    private void Blink(Skill skill, uint sourceUnit, mmo_shared.Vector2 target) {
        Player player = playerService.FindPlayer(sourceUnit);
        Vector3 particlePosition = player.transform.position + new Vector3(0, 1.5f, 0);
        if (player != null) {
            particleService.PlayParticles(blinkParticlesPrefab, particlePosition, 1f);
        }
    }
	
}
