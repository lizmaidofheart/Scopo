using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining = 5;
    public float radiusToFollowIn = 0;
    public bool chasing = false;

    public Follow(string name, CryptidBrain brain, float timeToLose, float radius = 0, bool chase = false) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        radiusToFollowIn = radius;
        chasing = chase;
        
    }

    public override void Enter()
    {
        base.Enter();

        timeRemaining = timeToLosePlayer;
        CryptidBrain.Instance.navigator.SetDestination(PlayerReference.Instance.transform.position);

        // subscribe to player taking photo
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if can sense player, update navigation destination and reset lose player timer
        if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            CryptidBrain.Instance.navigator.SetDestination(PlayerReference.Instance.transform.position);
            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;
        }
        // otherwise, reduce lose player timer
        else timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            // lost player

            brain.ChangeState("HuntNormal");
        }
    }

    public override void Exit()
    {
        base.Exit();

        // stop pathfinding
        CryptidBrain.Instance.navigator.ResetPath();
    }

    private void PlayerTakenPhoto(bool ofCryptid)
    {
        if (!ofCryptid) // player taken a photo that wasn't of the cryptid
        {
            CryptidBrain.Instance.curiosity += 1;
        }
        else // player taken a photo of the cryptid
        {
            // no effect rn
        }
    }
}
