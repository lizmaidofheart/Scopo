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
    public float sprintDuration = 5;
    public float restDuration = 3;
    public bool isResting = false;
    public float toggleSprintRestTimer;

    public Chase(string name, CryptidBrain brain, float time, float attackDist, float newSpeed, float sprintTime, float restTime) : base(name, brain)
    {
        timeToLosePlayer = time;
        attackDistance = attackDist;
        imposedSpeed = newSpeed;
        sprintDuration = sprintTime;
        restDuration = restTime;
    }

    public override void Enter()
    {
        base.Enter();

        defaultRadius = CryptidBrain.Instance.playerObstacle.radius;
        CryptidBrain.Instance.playerObstacle.radius = 1;

        defaultSpeed = CryptidBrain.Instance.navigator.speed;
        CryptidBrain.Instance.navigator.speed = imposedSpeed;

        toggleSprintRestTimer = sprintDuration;

        timeRemaining = timeToLosePlayer;
        EnableStareAtPlayer(true);

        CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

        AdaptiveMusic.Instance.SwitchTrack(4);
    }

    // sprints after player for a short time, then stops to rest for a short time, then sprints again; loop continues

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if can sense player, update navigation destination, reset lose player timer and set cryptid to stare at the player
        if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);
            EnableStareAtPlayer(true);
            
            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

        }
        // otherwise, reduce lose player timer and stop staring at the player
        else
        {
            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                brain.ChangeState("HuntAggressive");
            }

            EnableStareAtPlayer(false);
        }

        toggleSprintRestTimer -= Time.deltaTime;
        // if time has run out to toggle between sprinting and resting, toggle
        if (toggleSprintRestTimer <= 0)
        {
            if (isResting)
            {
                isResting = false;
                toggleSprintRestTimer = sprintDuration;
                CryptidBrain.Instance.navigator.speed = imposedSpeed;
            }
            else
            {
                isResting = true;
                toggleSprintRestTimer = restDuration;
                CryptidBrain.Instance.navigator.speed = 0;
            }
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
