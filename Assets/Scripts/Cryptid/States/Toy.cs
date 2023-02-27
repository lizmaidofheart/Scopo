using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toy : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining;
    public float aggressionIncreaseOnLosePlayer = 1;
    public float defendedZoneTimeBeforeRefreshAggression;
    public float aggressionToLurk = 10;
    public float curiosityToFollow = 5;
    public float interestingTime = 20;
    public float interestingTimeRemaining;

    public Toy(string name, CryptidBrain brain, float timeToLose, float aggressionIncrease, float defendedTime, float lurkAggression, float followCuriosity,
        float interesting) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        aggressionIncreaseOnLosePlayer = aggressionIncrease;
        defendedZoneTimeBeforeRefreshAggression = defendedTime;
        aggressionToLurk = lurkAggression;
        curiosityToFollow = followCuriosity;
        interestingTime = interesting;
    }

    public override void Enter()
    {
        base.Enter();
        timeRemaining = timeToLosePlayer;
        interestingTimeRemaining = interestingTime;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if aggression is high enough, swap to 'lurk' mode
        if (CryptidBrain.Instance.aggression >= aggressionToLurk)
        {
            brain.ChangeState("Lurk");
        }

        // if curiosity is low enough, swap to 'follow' mode
        else if (CryptidBrain.Instance.curiosity <= curiosityToFollow)
        {
            brain.ChangeState("Follow");
        }

        // if can sense player, update navigation destination, reset lose player timer and set cryptid to stare at the player
        else if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            // DO TOY STUFF

            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            EnableStareAtPlayer(true);

            DefendedZoneHandling(4, 2, 0, defendedZoneTimeBeforeRefreshAggression);

            interestingTimeRemaining -= Time.deltaTime;
            if (interestingTimeRemaining <= 0)
            {
                CryptidBrain.Instance.curiosity += -1;
            }
        }
        // otherwise, move towards last known player location, reduce lose player timer and stop staring at the player
        else
        {
            CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                CryptidBrain.Instance.aggression += aggressionIncreaseOnLosePlayer;
                brain.ChangeState("HuntNormal");
            }

            EnableStareAtPlayer(false);
        }
    }

    public override void CryptidPhotographed()
    {
        base.CryptidPhotographed();
        CryptidBrain.Instance.aggression += 5;
        interestingTimeRemaining = interestingTime;
    }

    public override void NotCryptidPhotographed()
    {
        base.NotCryptidPhotographed();
        CryptidBrain.Instance.curiosity += 1.5f;
        CryptidBrain.Instance.aggression += -1;
        interestingTimeRemaining = interestingTime;
    }
}
