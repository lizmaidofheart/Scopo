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
    public float watchedRefreshTime = 5;
    public float watchedTimeRemaining;
    public float loseInterestTime = 20;
    public float loseInterestTimeRemaining;

    public Lurk(string name, CryptidBrain brain, float timeToLose, float aggressionBump, float defendedTime, float chaseAggression, float distance, float watchDistance,
        float avoidance, float watchedTime, float interestTime) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        aggressionIncrease = aggressionBump;
        defendedZoneTimeBeforeRefreshAggression = defendedTime;
        aggressionToChase = chaseAggression;
        distanceToLurk = distance;
        distanceWhenBeingWatched = watchDistance;
        imposedAvoidance = avoidance;
        watchedRefreshTime = watchedTime;
        loseInterestTime = interestTime;
    }

    // use CryptidBrain.Instance.senses.PlayerCanSeeMe()

    public override void Enter()
    {
        base.Enter();
        timeRemaining = timeToLosePlayer;
        defaultAvoidance = CryptidBrain.Instance.navigator.radius;
        CryptidBrain.Instance.navigator.radius = imposedAvoidance;
        watchedTimeRemaining = watchedRefreshTime;
        loseInterestTimeRemaining = loseInterestTime;

        AdaptiveMusic.Instance.SwitchTrack(2);
        CryptidBrain.Instance.animator.SetBool("isWalking", true);
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

            LurkBehaviour();

            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                brain.ChangeState("HuntAggressive");
            }

        }
    }

    public override void Exit()
    {
        base.Exit();
        CryptidBrain.Instance.navigator.radius = defaultAvoidance;
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

            Vector3 perpendicular = Vector3.Cross(PlayerReference.Instance.transform.forward, CryptidBrain.Instance.body.position - PlayerReference.Instance.transform.position);
            float dir = Vector3.Dot(perpendicular, Vector3.up);

            if (dir > 0f)
            {
                positionToLurk = CryptidBrain.Instance.senses.lastKnownPlayerLocation + PlayerReference.Instance.transform.right * distanceWhenBeingWatched;
            }
            else
            {
                positionToLurk = CryptidBrain.Instance.senses.lastKnownPlayerLocation - PlayerReference.Instance.transform.right * distanceWhenBeingWatched;
            }

            // if player is looking at the cryptid, handle timers
            watchedTimeRemaining -= Time.deltaTime;

            if (watchedTimeRemaining <= 0)
            {
                watchedTimeRemaining = watchedRefreshTime;
                CryptidBrain.Instance.aggression += aggressionIncrease;
            }

            loseInterestTimeRemaining = loseInterestTime;
        }
        else
        {
            // if player can't see the cryptid, stands directly behind the player at distanceToLurk distance and handle loseInterestTime

            positionToLurk = CryptidBrain.Instance.senses.lastKnownPlayerLocation + PlayerReference.Instance.transform.forward * -distanceToLurk;

            // time reduces slower if aggression is high

            float aggressionFactor = 1 - (CryptidBrain.Instance.aggression - 10) / 20;
            loseInterestTimeRemaining -= Time.deltaTime * aggressionFactor;

            if (loseInterestTimeRemaining <= 0)
            {
                CryptidBrain.Instance.aggression = 8;
                if (CryptidBrain.Instance.curiosity >= 6) brain.ChangeState("Toy");
                else brain.ChangeState("Follow");
            }
        }

        // make sure that location is valid (reachable)
        if (FindValidLocation(positionToLurk, PlayerReference.Instance.transform.position, out Vector3 destination))
        {
            CryptidBrain.Instance.navigator.SetDestination(destination);
        }
        
        EnableStareAtPlayer(true);
    }

}
