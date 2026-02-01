using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeBaseState : State
{
    // How long minimum the attack must play before it CAN combo
    public float duration;
    
    protected Animator animator;
    protected bool shouldCombo;
    protected int attackIndex;

    protected Collider2D hitCollider;
    private List<Collider2D> collidersDamaged;
    private GameObject HitEffectPrefab;

    // Input buffer Timer
    public float AttackPressedTimer = 0;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        if (PlayerController.instance != null)
        {
            animator = PlayerController.instance.currentAnimator;
        }
        else
        {
            Debug.LogError("PlayerController Instance not found!");
        }
        collidersDamaged = new List<Collider2D>();
        
        // Ensure references are valid to prevent errors
        var playerController = GetComponent<PlayerController>();
        if(playerController != null) {
            hitCollider = playerController.hitbox;
            HitEffectPrefab = playerController.Hiteffect;
        }
        
        // Reset combo flag on entry
        shouldCombo = false; 
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        // Assuming fixedtime is defined in base State and reset to 0 on OnEnter
        fixedtime += Time.deltaTime; 
        AttackPressedTimer -= Time.deltaTime;
        
        // CHECK 1: Register the Combo Input
        float window = animator.GetFloat("AttackWindow.Open");
        
        // We check if the window is open AND we have a buffered input
        if (window > 0f && AttackPressedTimer > 0)
        {
            shouldCombo = true;
            Debug.Log("Combo Window Success! shouldCombo is now true.");
            // Optional: Consume the timer so it doesn't trigger twice
            AttackPressedTimer = 0;
            OnComboTriggered(); 
        }
        if (IsAnimationFinished() && !shouldCombo)
        {
            stateMachine.SetNextStateToMain();
        }
        // CHECK 2: Perform the Hit Detection
        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }
    }

    // Helper to check if the Animation is completely finished (NormalizedTime > 1)
    protected bool IsAnimationFinished()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }

    protected void Attack()
    {
        Debug.Log("attacking left click");
        // ... (Your existing Attack logic remains unchanged) ...
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        if (hitCollider == null) return; // Safety Check

        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (hitTeamComponent != null && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    GameObject.Instantiate(HitEffectPrefab, collidersToDamage[i].transform.position, Quaternion.identity);
                    EnemyHealth enemyHealth = hitTeamComponent.GetComponent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.Takedamage(attackIndex, 1);
                        Debug.Log($"Enemy hit! Damage: {attackIndex}");
                    }
                    collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }
    }

    protected virtual void OnComboTriggered() { }
}