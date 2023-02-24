using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BrainState
{
    public float idleTime;
    public float idleTimeRemaining;
    public BrainState nextState;

    public Idle(CryptidBrain brain, float time, BrainState next) : base("Idle", brain)
    {
        idleTime = time;
        nextState = next;
    }

    public override void Enter()
    {
        base.Enter();
        idleTimeRemaining = idleTime;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        idleTimeRemaining -= Time.deltaTime;
        if (idleTimeRemaining <= 0)
        {
            brain.ChangeState(nextState);
        }

    }
}
