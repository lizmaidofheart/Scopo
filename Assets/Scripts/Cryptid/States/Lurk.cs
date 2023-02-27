using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lurk : BrainState
{
    public float timeToLosePlayer = 5;
    public float timeRemaining;
    public float aggressionIncrease = 1;
    public float defendedZoneTimeBeforeRefreshAggression;
    public float aggressionToChase;
    public float distanceToLurk = 5;
    public float distanceWhenBeingWatched = 15;
    public float defaultAvoidance;
    public float imposedAvoidance = 2;
    public float defaultSpeed;
    public float imposedSpeed = 5;
    public float watchedRefreshTime = 5;
    public float watchedTimeRemaining;

    public Lurk(string name, CryptidBrain brain, float timeToLose, float aggressionBump, float defendedTime, float chaseAggression, float distance, float watchDistance,
        float avoidance, float speed, float watchedTime) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        aggressionIncrease = aggressionBump;
        defendedZoneTimeBeforeRefreshAggression = defendedTime;
        aggressionToChase = chaseAggression;
        distanceToLurk = distance;
        distanceWhenBeingWatched = watchDistance;
        imposedAvoidance = avoidance;
        imposedSpeed = speed;
        watchedRefreshTime = watchedTime;
    }

    // use CryptidBrain.Instance.senses.PlayerCanSeeMe()

    public override void Enter()
    {
        base.Enter();
        timeRemaining = timeToLosePlayer;
        defaultAvoidance = CryptidBrain.Instance.navigator.radius;
        CryptidBrain.Instance.navigator.radius = imposedAvoidance;
        defaultSpeed = CryptidBrain.Instance.navigator.speed;
        CryptidBrain.Instance.navigator.speed = imposedSpeed;
        watchedTimeRemaining = 2;
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
            LurkBehaviour();

            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            DefendedZoneHandling(5, 2, 0, defendedZoneTimeBeforeRefreshAggression);

        }
        // otherwise, move towards last known player location, reduce lose player timer and stop staring at the player
        else
        {
            //CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            LurkBehaviour();

            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                CryptidBrain.Instance.aggression += aggressionIncrease;
                brain.ChangeState("HuntAggressive");
            }

            //EnableStareAtPlayer(false);
        }
    }

    public override void Exit()
    {
        base.Exit();
        CryptidBrain.Instance.navigator.radius = defaultAvoidance;
        CryptidBrain.Instance.navigator.speed = defaultSpeed;
    }

    public override void CryptidPhotographed()
    {
        base.CryptidPhotographed();
        CryptidBrain.Instance.aggression += 5;
    }

    private void LurkBehaviour()
    {
        Vector3 positionToLurk;

        if (CryptidBrain.Instance.senses.PlayerCanSeeMe())
        {
            // if player can see the cryptid, move away to a far distance and try move out of the player's vision - if handles whether to go left or right
            // this code is adapted from https://forum.unity.com/threads/left-right-test-function.31420/

            Vector3 perp = Vector3.Cross(PlayerReference.Instance.transform.forward, CryptidBrain.Instance.body.position - PlayerReference.Instance.transform.position);
            float dir = Vector3.Dot(perp, Vector3.up);

            if (dir > 0f)
            {
                positionToLurk = CryptidBrain.Instance.senses.lastKnownPlayerLocation + PlayerReference.Instance.transform.right * distanceWhenBeingWatched;
            }
            else
            {
                positionToLurk = CryptidBrain.Instance.senses.lastKnownPlayerLocation - PlayerReference.Instance.transform.right * distanceWhenBeingWatched;
            }

            // if player is looking at the cryptid, handle reducing watch timer
            watchedTimeRemaining -= Time.deltaTime;

            if (watchedTimeRemaining <= 0)
            {
                watchedTimeRemaining = watchedRefreshTime;
                CryptidBrain.Instance.aggression += aggressionIncrease;
            }
        }
        else
        {
            // if player can't see the cryptid, stands directly behind the player at distanceToLurk distance

            positionToLurk = CryptidBrain.Instance.senses.lastKnownPlayerLocation + PlayerReference.Instance.transform.forward * -distanceToLurk;
        }

        // make sure that location is valid (reachable)
        if (FindValidLocation(positionToLurk, PlayerReference.Instance.transform.position, out Vector3 destination))
        {
            CryptidBrain.Instance.navigator.SetDestination(destination);
        }
        
        EnableStareAtPlayer(true);
    }

}
