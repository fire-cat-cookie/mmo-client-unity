using mmo_shared.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject autoAttackHitAnimation;

    public CharInfo CharInfo { get; set; }
    public Vector3 Destination { get; set; }
    public float AttackCooldownRemaining { get; set; }
    public bool InterruptAttack { get; set; }
    public Player CurrentAttackTarget { get; set; }

    private Animator animator;
    private PlayerMovement movement;
    private ParticleService particleService;

    private IEnumerator attackCoroutine;

    void Awake() {
        animator = GetComponentInChildren<Animator>();
        movement = GetComponentInChildren<PlayerMovement>();
        particleService = FindObjectOfType<ParticleService>();
    }

    void Update() {
        //reduce the remaining attack cooldown once the attack animation has finished.
        if (!InAttackAnimation()) {
            AttackCooldownRemaining = Mathf.Clamp(AttackCooldownRemaining - Time.deltaTime, 0, float.MaxValue);
        }
    }

    public void SetDestination(Vector3 target) {
        Destination = target;
        if (Destination != transform.position) {
            animator.Play("Run");
        } else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run")){
            animator.Play("Idle_A");
        }
    }

    public void QueueAttack(Player target) {
        if (target == CurrentAttackTarget) {
            return;
        }
        
        InterruptAttack = false;
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = AttackWhenPossible(target);
        StartCoroutine(attackCoroutine);
    }

    public bool InAttackAnimation() {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    public void Die() {
        animator.Play("Die");
    }

    public void Revive() {
        animator.Play("Idle_A");
    }

    private void Attack(Player target) {
        AttackCooldownRemaining = CharInfo.AttackCooldown;
        transform.LookAt(target.transform.position);
        animator.Play("Attack");
        StartCoroutine(SpawnAttackParticles(target));
    }

    private IEnumerator SpawnAttackParticles(Player target) {
        yield return null; //wait one frame so that the animator can transition to the attack animation
        while (InAttackAnimation()) {
            yield return null;
        }
        if (CurrentAttackTarget == null) {
            yield break;
        }
        particleService.PlayParticles(autoAttackHitAnimation, target.transform.position, 0.5f);
    }

    private IEnumerator AttackWhenPossible(Player target) {
        CurrentAttackTarget = target;
        while (!InterruptAttack) {
            if (CanAttack(target)) {
                Attack(target);
            }
            yield return null;
        }
        CurrentAttackTarget = null;
        InterruptAttack = false;
    }

    private bool CanAttack(Player target) {
        return target != null && InAttackRange(target) && AttackCooldownRemaining <= 0;
    }

    private bool InAttackRange(Player target) {
        var targetPosition = new mmo_shared.Vector2(target.transform.position.x, target.transform.position.z);
        return CharInfo.Position.Distance(targetPosition) <= CharInfo.AttackRange;
    }

}
