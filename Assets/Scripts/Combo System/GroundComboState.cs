using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundComboState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 2;
        duration = 0.4f;
        animator.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (shouldCombo && fixedtime >= duration)
        {
            stateMachine.SetNextState(new GroundFinisherState());
        }
        else if (IsAnimationFinished())
        {
            stateMachine.SetNextStateToMain();
        }
    }
}