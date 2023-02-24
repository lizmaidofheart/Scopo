using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BrainState
{
    public float idleTime;
    public float idleTimeRemaining;
    public string nextStateKey;

    public Idle(CryptidBrain brain, float time, string next) : base("Idle", brain)
    {
        idleTime = time;
        nextStateKey = next;
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
            brain.ChangeState(nextStateKey);
        }

    }
}
