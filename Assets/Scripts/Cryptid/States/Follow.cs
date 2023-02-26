using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining = 5;
    public float aggressionIncreaseOnLosePlayer = 1;
    public float curiosityToToy = 6;
    public float aggressionToLurk = 10;
    public float radiusToFollowIn = 0;
    

    public Follow(string name, CryptidBrain brain, float timeToLose, float aggressionIncrease, float radius, float curiosity, float aggression) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        aggressionIncreaseOnLosePlayer = aggressionIncrease;
        radiusToFollowIn = radius;
        curiosityToToy = curiosity;
        aggressionToLurk = aggression;
    }

    public override void Enter()
    {
        base.Enter();

        timeRemaining = timeToLosePlayer;
        EnableStareAtPlayer(true);

        CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if curiosity is high enough, swap to 'toy' mode
        if (CryptidBrain.Instance.curiosity >= curiosityToToy)
        {
            brain.ChangeState("Toy");
        }

        // if aggression is high enough, swap to 'lurk' mode
        else if (CryptidBrain.Instance.aggression >= aggressionToLurk)
        {
            brain.ChangeState("Lurk");
        }

        // if can sense player, update navigation destination, reset lose player timer and set cryptid to stare at the player
        else if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            if (radiusToFollowIn != 0) CryptidBrain.Instance.navigator.SetDestination(closestPositionAtFollowDistance());
            else CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            EnableStareAtPlayer(true);
        }
        // otherwise, move towards last known player location, reduce lose player timer and stop staring at the player
        else
        {
            CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                CryptidBrain.Instance.aggression += 1;
                brain.ChangeState("HuntNormal");
            }

            EnableStareAtPlayer(false);
        }
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
        // returns the closest position where the distance from the player to the cryptid is equal to the set follow radius

        Vector3 angleFromPlayerToCryptid = (CryptidBrain.Instance.body.position - CryptidBrain.Instance.senses.lastKnownPlayerLocation).normalized;
        return CryptidBrain.Instance.senses.lastKnownPlayerLocation + angleFromPlayerToCryptid * radiusToFollowIn;
    }
}
