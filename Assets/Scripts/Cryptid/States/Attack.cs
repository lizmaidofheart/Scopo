using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BrainState
{
    public float duration = 2;
    public float timeRemaining;

    public Attack(string name, CryptidBrain brain, float animationDuration) : base(name, brain)
    {
        duration = animationDuration;
    }

    public override void Enter()
    {
        base.Enter();
        // play attack animation
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            // reset
        }
    }

}
