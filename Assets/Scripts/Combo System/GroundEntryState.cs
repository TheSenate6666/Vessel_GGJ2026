using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEntryState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 1;
        duration = 0.4f;
        animator.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 1. Check for Combo Transition
        // We can transition as soon as duration is passed OR animation is near end
        //if (shouldCombo && fixedtime >= duration)
        //{
           // stateMachine.SetNextState(new GroundComboState());
        //}
        // 2. Check for Return to Idle
        // ONLY return to idle if the animation has actually finished playing.
        // This prevents the state from cutting off early if duration is too short.
        if (IsAnimationFinished())
        {
            stateMachine.SetNextStateToMain();
        }
    }

    protected override void OnComboTriggered()
    {
        // No waiting for duration—just go!
        stateMachine.SetNextState(new GroundComboState());
    }
}