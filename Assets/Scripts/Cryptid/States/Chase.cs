using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining = 5;
    public float attackDistance = 3;
    public bool playerInDefendedZone = false;
    public float aggressionReduction = 4;
    public float defaultRadius;
    public float defaultSpeed;
    public float imposedSpeed;

    public Chase(string name, CryptidBrain brain, float time, float attackDist, float newSpeed) : base(name, brain)
    {
        timeToLosePlayer = time;
        attackDistance = attackDist;
        imposedSpeed = newSpeed;
    }

    public override void Enter()
    {
        base.Enter();

        defaultRadius = CryptidBrain.Instance.playerObstacle.radius;
        CryptidBrain.Instance.playerObstacle.radius = 1;

        defaultSpeed = CryptidBrain.Instance.navigator.speed;
        CryptidBrain.Instance.navigator.speed = imposedSpeed;

        timeRemaining = timeToLosePlayer;
        EnableStareAtPlayer(true);

        CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if can sense player, update navigation destination, reset lose player timer and set cryptid to stare at the player
        if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            EnableStareAtPlayer(true);
        }
        // otherwise, reduce lose player timer and stop staring at the player
        else
        {
            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                CryptidBrain.Instance.aggression += 1;
                brain.ChangeState("HuntAggressive");
            }

            EnableStareAtPlayer(false);
        }

        DefendedZoneHandling(0, 0, aggressionReduction, -1);

        // if close enough to player, attack them
        if ((PlayerReference.Instance.transform.position - CryptidBrain.Instance.body.position).magnitude <= attackDistance) brain.ChangeState("Attack");
    }

    public override void Exit()
    {
        base.Exit();
        CryptidBrain.Instance.playerObstacle.radius = defaultRadius;
        CryptidBrain.Instance.navigator.speed = defaultSpeed;
    }

    public override void CryptidPhotographed()
    {
        base.CryptidPhotographed();
        CryptidBrain.Instance.aggression += 5;
    }
}
