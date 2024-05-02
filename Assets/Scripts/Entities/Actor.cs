using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public uint ActorId { get; set; }
    public mmo_shared.Vector2 Position { get; set; }
    public Vector3 Destination { get; set; }
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float AttackRange { get; set; }
    public float AttackCooldown { get; set; }
    public float AttackDamage { get; set; }
    public float MoveSpeed { get; set; }
    public bool Alive { get; set; }
    public uint ZoneId { get; set; }
    public string Name { get; set; }

    public GameObject autoAttackHitAnimation;
    public float AttackCooldownRemaining { get; set; }
    public bool InterruptAttack { get; set; }
    public Actor CurrentAttackTarget { get; set; }

    private Animator animator;
    private PlayerMovement movement;
    private Actor actor;
    private ParticleService particleService;

    private IEnumerator attackCoroutine;

    void Awake()
    {
        actor = GetComponent<Actor>();
        animator = GetComponentInChildren<Animator>();
        movement = GetComponentInChildren<PlayerMovement>();
        particleService = FindObjectOfType<ParticleService>();
    }

    void Update()
    {
        //reduce the remaining attack cooldown once the attack animation has finished.
        if (!InAttackAnimation())
        {
            AttackCooldownRemaining = Mathf.Clamp(AttackCooldownRemaining - Time.deltaTime, 0, float.MaxValue);
        }
    }

    public void SetDestination(Vector3 target)
    {
        actor.Destination = target;
        if (actor.Destination != transform.position)
        {
            animator.Play("Run");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            animator.Play("Idle_A");
        }
    }

    public void QueueAttack(Actor target)
    {
        if (target == CurrentAttackTarget)
        {
            return;
        }

        InterruptAttack = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = AttackWhenPossible(target);
        StartCoroutine(attackCoroutine);
    }

    public bool InAttackAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    public void Die()
    {
        animator.Play("Die");
    }

    public void Revive()
    {
        animator.Play("Idle_A");
    }

    private void Attack(Actor target)
    {
        AttackCooldownRemaining = AttackCooldown;
        transform.LookAt(target.transform.position);
        animator.Play("Attack");
        StartCoroutine(SpawnAttackParticles(target));
    }

    private IEnumerator SpawnAttackParticles(Actor target)
    {
        yield return null; //wait one frame so that the animator can transition to the attack animation
        while (InAttackAnimation())
        {
            yield return null;
        }
        if (CurrentAttackTarget == null)
        {
            yield break;
        }
        particleService.PlayParticles(autoAttackHitAnimation, target.transform.position, 0.5f);
    }

    private IEnumerator AttackWhenPossible(Actor target)
    {
        CurrentAttackTarget = target;
        while (!InterruptAttack)
        {
            if (CanAttack(target))
            {
                Attack(target);
            }
            yield return null;
        }
        CurrentAttackTarget = null;
        InterruptAttack = false;
    }

    private bool CanAttack(Actor target)
    {
        return target != null && InAttackRange(target) && AttackCooldownRemaining <= 0;
    }

    private bool InAttackRange(Actor target)
    {
        var targetPosition = new mmo_shared.Vector2(target.transform.position.x, target.transform.position.z);
        var subjectPosition = new mmo_shared.Vector2(transform.position.x, transform.position.z);
        return subjectPosition.Distance(targetPosition) <= AttackRange;
    }
}
