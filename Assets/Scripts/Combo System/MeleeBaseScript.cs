using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeBaseState : State
{
    // How long this state should be active for before moving on
    public float duration;
    // Cached animator component
    protected Animator animator;
    // bool to check whether or not the next attack in the sequence should be played or not
    protected bool shouldCombo;
    // The attack index in the sequence of attacks
    protected int attackIndex;



    // The cached hit collider component of this attack
    protected Collider2D hitCollider;
    // Cached already struck objects of said attack to avoid overlapping attacks on same target
    private List<Collider2D> collidersDamaged;
    // The Hit Effect to Spawn on the afflicted Enemy
    private GameObject HitEffectPrefab;

    // Input buffer Timer
    public float AttackPressedTimer = 0;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator = GetComponent<Animator>();
        collidersDamaged = new List<Collider2D>();
        hitCollider = GetComponent<PlayerController>().hitbox;
        HitEffectPrefab = GetComponent<PlayerController>().Hiteffect;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        fixedtime += Time.deltaTime;
        AttackPressedTimer -= Time.deltaTime;
        
        float window = animator.GetFloat("AttackWindow.Open");
    
        if (window > 0f && AttackPressedTimer > 0)
        {
            shouldCombo = true;
            Debug.Log("Combo Window Success! shouldCombo is now true.");
        }

        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }


        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
        {
            shouldCombo = true;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    protected void Attack()
{
    Collider2D[] collidersToDamage = new Collider2D[10];
    ContactFilter2D filter = new ContactFilter2D();
    filter.useTriggers = true;

    // hitCollider should be defined in your class scope
    int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

    for (int i = 0; i < colliderCount; i++)
    {
        if (!collidersDamaged.Contains(collidersToDamage[i]))
        {
            TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

            // 1. Verify Team and Component
            if (hitTeamComponent != null && hitTeamComponent.teamIndex == TeamIndex.Enemy)
            {
                // 2. Instantiate VFX
                GameObject.Instantiate(HitEffectPrefab, collidersToDamage[i].transform.position, Quaternion.identity);
                
                // 3. Get the Health Script safely
                EnemyHealth enemyHealth = hitTeamComponent.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    int dmgMult = 1;
                    // Note: Use the variable 'enemyHealth', not the Class name 'EnemyHealth'
                    enemyHealth.Takedamage(attackIndex, dmgMult);
                    
                    Debug.Log($"Enemy hit! Damage: {attackIndex * dmgMult}");
                }

                collidersDamaged.Add(collidersToDamage[i]);
            }
        }
    }
}

}
