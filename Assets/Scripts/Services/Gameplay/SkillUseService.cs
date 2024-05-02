using mmo_shared;
using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUseService : MonoBehaviour {

    private PacketPublisher packetPublisher;

    public delegate void HandleNoTargetSkill(Skill skill, uint sourceUnit);
    public delegate void HandleGroundTargetSkill(Skill skill, uint sourceUnit, mmo_shared.Vector2 target);
    public delegate void HandleUnitTargetSkill(Skill skill, uint sourceUnit, uint target);

    private Dictionary<string, List<HandleNoTargetSkill>> noTargetHandlers = new Dictionary<string, List<HandleNoTargetSkill>>();
    private Dictionary<string, List<HandleGroundTargetSkill>> groundTargetHandlers = new Dictionary<string, List<HandleGroundTargetSkill>>();
    private Dictionary<string, List<HandleUnitTargetSkill>> unitTargetHandlers = new Dictionary<string, List<HandleUnitTargetSkill>>();

	void Awake(){
        packetPublisher = FindObjectOfType<PacketPublisher>();
        packetPublisher.Subscribe(typeof(ServerGroundTargetSkill), HandleGroundTarget);
        packetPublisher.Subscribe(typeof(ServerUnitTargetSkill), HandleUnitTarget);
        packetPublisher.Subscribe(typeof(ServerNoTargetSkill), HandleNoTarget);
    }
	
    private void HandleGroundTarget(Message message) {
        ServerGroundTargetSkill skillUse = message as ServerGroundTargetSkill;
        if (!VerifySkill(skillUse.SkillId, out Skill skill)) {
            return;
        }

        if (groundTargetHandlers.ContainsKey(skill.CodeName)) {
            foreach (var handler in groundTargetHandlers[skill.CodeName]) {
                handler.Invoke(skill, skillUse.SourceUnitId, skillUse.Target);
            }
        }
    }

    private void HandleUnitTarget(Message message) {
        ServerUnitTargetSkill skillUse = message as ServerUnitTargetSkill;
        if (!VerifySkill(skillUse.SkillId, out Skill skill)) {
            return;
        }

        if (unitTargetHandlers.ContainsKey(skill.CodeName)) {
            foreach (var handler in unitTargetHandlers[skill.CodeName]) {
                handler.Invoke(skill, skillUse.SourceUnitId, skillUse.Target);
            }
        }
    }

    private void HandleNoTarget(Message message) {
        ServerNoTargetSkill skillUse = message as ServerNoTargetSkill;
        if (!VerifySkill(skillUse.SkillId, out Skill skill)) {
            return;
        }

        if (noTargetHandlers.ContainsKey(skill.CodeName)) {
            foreach (var handler in noTargetHandlers[skill.CodeName]) {
                handler.Invoke(skill, skillUse.SourceUnitId);
            }
        }
    }

    private bool VerifySkill(uint skillId, out Skill skill) {
        skill = null;
        if (skillId > SkillData.skills.Length - 1) {
            return false;
        }
        skill = SkillData.skills[skillId];
        return true;
    }

    public void Subscribe(string skillName, HandleNoTargetSkill handler) {
        if (!noTargetHandlers.ContainsKey(skillName)) {
            noTargetHandlers[skillName] = new List<HandleNoTargetSkill>();
        }
        noTargetHandlers[skillName].Add(handler);
    }

    public void Subscribe(string skillName, HandleGroundTargetSkill handler) {
        if (!groundTargetHandlers.ContainsKey(skillName)) {
            groundTargetHandlers[skillName] = new List<HandleGroundTargetSkill>();
        }
        groundTargetHandlers[skillName].Add(handler);
    }

    public void Subscribe(string skillName, HandleUnitTargetSkill handler) {
        if (!unitTargetHandlers.ContainsKey(skillName)) {
            unitTargetHandlers[skillName] = new List<HandleUnitTargetSkill>();
        }
        unitTargetHandlers[skillName].Add(handler);
    }
}
