using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lurk : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining;
    public float aggressionIncreaseOnLosePlayer = 1;
    public float defendedZoneTimeBeforeRefreshAggression;
    public float aggressionToChase;

    public Lurk(string name, CryptidBrain brain, float timeToLose, float aggressionIncrease, float defendedTime, float chaseAggression) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        aggressionIncreaseOnLosePlayer = aggressionIncrease;
        defendedZoneTimeBeforeRefreshAggression = defendedTime;
        aggressionToChase = chaseAggression;

    }

    // use CryptidBrain.Instance.senses.PlayerCanSeeMe()

    public override void Enter()
    {
        base.Enter();
        timeRemaining = timeToLosePlayer;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if aggression is high enough, swap to 'chase' mode
        if (CryptidBrain.Instance.aggression >= aggressionToChase)
        {
            brain.ChangeState("Chase");
        }

        // if can sense player, update navigation destination, reset lose player timer and set cryptid to stare at the player
        else if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            // DO TOY STUFF

            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            EnableStareAtPlayer(true);

            DefendedZoneHandling(5, 2, 0, defendedZoneTimeBeforeRefreshAggression);

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
                brain.ChangeState("HuntAggressive");
            }

            EnableStareAtPlayer(false);
        }
    }

    public override void CryptidPhotographed()
    {
        base.CryptidPhotographed();
        CryptidBrain.Instance.aggression += 5;
    }

}
