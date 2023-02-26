using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining = 5;
    public float radiusToFollowIn = 0;
    public bool chasing = false;
    public bool stareAtPlayer = true;

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

        if (radiusToFollowIn != 0) CryptidBrain.Instance.navigator.SetDestination(closestPositionAtFollowDistance());
        else CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if can sense player, update navigation destination, reset lose player timer and set cryptid to stare at the player
        if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            if (radiusToFollowIn != 0) CryptidBrain.Instance.navigator.SetDestination(closestPositionAtFollowDistance());
            else CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            if (!stareAtPlayer)
            {
                stareAtPlayer = true;
                CryptidBrain.Instance.navigator.angularSpeed = 0;
            }
        }
        // otherwise, move towards last known player location, reduce lose player timer and stop staring at the player
        else
        {
            CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                // lost player
                brain.ChangeState("HuntNormal");
            }

            if (stareAtPlayer)
            {
                stareAtPlayer = false;
                CryptidBrain.Instance.navigator.angularSpeed = 120;
            }
        }

        if (stareAtPlayer) StareAtPlayer();
    }

    public override void CryptidPhotographed()
    {
        base.CryptidPhotographed();
        CryptidBrain.Instance.aggression += 5;
    }

    public override void NotCryptidPhotographed()
    {
        base.NotCryptidPhotographed();
        CryptidBrain.Instance.curiosity += 1;
    }

    private Vector3 closestPositionAtFollowDistance()
    {
        Vector3 angleFromPlayerToCryptid = (CryptidBrain.Instance.body.position - CryptidBrain.Instance.senses.lastKnownPlayerLocation).normalized;
        return angleFromPlayerToCryptid * radiusToFollowIn;
    }
}
