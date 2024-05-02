using mmo_shared;
using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInputService : MonoBehaviour{

    private InputHandler inputHandler;
    private MessageSender messageSender;

    private uint[] skillHotkeysToIds = new uint[] {
        1, 0
    };

    private Dictionary<Skill, float> activeCooldowns = new Dictionary<Skill, float>();
    
	void Awake() {
        inputHandler = FindObjectOfType<InputHandler>();
        messageSender = FindObjectOfType<MessageSender>();
        inputHandler.SkillPress += OnSkillPress;
	}

    void Update() {
        for (int i=0; i<skillHotkeysToIds.Length; i++) {
            Skill skill = SkillData.skills[skillHotkeysToIds[i]];
            if (activeCooldowns.ContainsKey(skill)) {
                activeCooldowns[skill] = Mathf.Max(0, activeCooldowns[skill] - Time.deltaTime);
            }
        }
    }
	
    private void OnSkillPress(uint skillSlot) {
        uint skillId = skillHotkeysToIds[skillSlot];
        Skill skill = SkillData.skills[skillId];
        if (OnCooldown(skill)) {
            return;
        }
        if (skill.SkillType == SkillType.GroundTarget) {
            GroundSkill(skillId);
        } else if (skill.SkillType == SkillType.UnitTarget) {
            UnitSkill(skillId);
        } else if (skill.SkillType == SkillType.NoTarget) {
            NoTargetSkill(skillId);
        }
    }

    private void GroundSkill(uint skillId) {
        messageSender.Send(new GroundTargetSkill(skillId, inputHandler.GetMousePositionOnTerrain()));
        StartCooldown(SkillData.skills[skillId]);
    }

    private void UnitSkill(uint skillId) {

    }

    private void NoTargetSkill(uint skillId) {

    }

    public Skill GetAssignedSkill(int slot) {
        return SkillData.skills[skillHotkeysToIds[slot]];
    }

    public float GetCurrentCooldown(Skill skill) {
        if (activeCooldowns.ContainsKey(skill)) {
            return activeCooldowns[skill];
        }
        return 0;
    }

    public bool OnCooldown(Skill skill) {
        return activeCooldowns.ContainsKey(skill) && activeCooldowns[skill] > 0;
    }

    private void StartCooldown(Skill skill) {
        activeCooldowns[skill] = skill.Cooldown;
    }


	
}
