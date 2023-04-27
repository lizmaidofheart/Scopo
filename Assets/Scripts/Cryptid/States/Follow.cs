using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining = 5;
    public float curiosityToToy = 6;
    public float aggressionToLurk = 10;
    public float radiusToFollowIn = 0;
    public bool playerInDefendedZone = false;
    public float defendedZoneTimeBeforeRefreshAggression = 10;


    public Follow(string name, CryptidBrain brain, float timeToLose, float radius, float curiosity, float aggression, float defenceTime) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        radiusToFollowIn = radius;
        curiosityToToy = curiosity;
        aggressionToLurk = aggression;
        defendedZoneTimeBeforeRefreshAggression = defenceTime;
    }

    public override void Enter()
    {
        base.Enter();

        timeRemaining = timeToLosePlayer;
        EnableStareAtPlayer(true);

        CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

        AdaptiveMusic.Instance.SwitchTrack(1);
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
            if (radiusToFollowIn != 0)
            {
                // find the position to follow to and make sure it's valid
                if (FindValidLocation(closestPositionAtFollowDistance(), PlayerReference.Instance.transform.position, out Vector3 destination))
                    CryptidBrain.Instance.navigator.SetDestination(destination);

                else CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);
            }
                
            else CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            EnableStareAtPlayer(true);

            DefendedZoneHandling(4, 2, 0, defendedZoneTimeBeforeRefreshAggression);
        }
        // otherwise, move towards last known player location, reduce lose player timer and stop staring at the player
        else
        {
            CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                brain.ChangeState("HuntNormal");
            }

            EnableStareAtPlayer(false);
        }
    }

    public override void CryptidPhotographed()
    {
        base.CryptidPhotographed();
        CryptidBrain.Instance.aggression += 3.5f;
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
