using mmo_shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewHandler : MonoBehaviour {

    public SimpleProjectile projectilePrefab;

    private SkillUseService skillUseService;
    private PlayerService playerService;

    void Awake() {
        skillUseService = FindObjectOfType<SkillUseService>();
        playerService = FindObjectOfType<PlayerService>();
        skillUseService.Subscribe("Pew", Pew);
    }

    private void Pew(Skill skill, uint sourceUnit, mmo_shared.Vector2 target) {
        Player player = playerService.FindPlayer(sourceUnit);
        if (player != null) {
            Vector3 targetPos = new Vector3(target.X, player.transform.position.y, target.Y);
            player.transform.LookAt(targetPos);
            SimpleProjectile projectile = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
            projectile.gameObject.transform.LookAt(targetPos);
            projectile.gameObject.transform.Translate(0, 0.5f, 0, Space.World);
            projectile.Direction = (targetPos - player.transform.position).normalized;
            projectile.Velocity = 20f;
            projectile.RemainingLifespan = 15f / projectile.Velocity;
            projectile.CollisionHandler = (Player hit) => { if (hit != player && hit.CharInfo.Alive) Destroy(projectile.gameObject); };
        }
    }

}
