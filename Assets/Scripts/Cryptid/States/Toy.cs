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
    public float cameraMovement = 0;
    public float cameraMovementThreshhold = 15;
    public float targetSelectionMaxDistance = 30;
    public float timeForSelectNewGoal = 30;
    public float goalTimeRemaining;
    public List<Transform> targets = new List<Transform>();
    public Vector3 currentGoalZone;
    public float maxDistanceBeforeKite = 50;
    public float kiteOffsetDistance = 5;

    public Toy(string name, CryptidBrain brain, float timeToLose, float aggressionIncrease, float defendedTime, float lurkAggression, float followCuriosity,
        float interesting, float camThreshhold, Transform kiteTargetsParent, float selectionMaxDistance, float newSelectionTime, float distanceToKite,
        float kiteOffset) : base(name, brain)
    {
        timeToLosePlayer = timeToLose;
        aggressionIncreaseOnLosePlayer = aggressionIncrease;
        defendedZoneTimeBeforeRefreshAggression = defendedTime;
        aggressionToLurk = lurkAggression;
        curiosityToFollow = followCuriosity;
        interestingTime = interesting;
        cameraMovementThreshhold = camThreshhold;
        targetSelectionMaxDistance = selectionMaxDistance;
        timeForSelectNewGoal = newSelectionTime;
        maxDistanceBeforeKite = distanceToKite;
        kiteOffsetDistance = kiteOffset;

        for (int i = 0; i < kiteTargetsParent.childCount; i++)
        {
            targets.Add(kiteTargetsParent.GetChild(i));
        }
    }

    public override void Enter()
    {
        base.Enter();
        timeRemaining = timeToLosePlayer;
        interestingTimeRemaining = interestingTime;
        goalTimeRemaining = 0;

        AdaptiveMusic.Instance.SwitchTrack(1);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // measure how shaky the player's camera is this frame
        cameraMovement += measureCameraMovement();

        // behaviour
        ToyBehaviour();
        EnableStareAtPlayer(true);

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

        // if can sense player, update navigation destination + reset lose player timer
        else if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            if (timeRemaining < timeToLosePlayer) timeRemaining = timeToLosePlayer;

            DefendedZoneHandling(4, 2, 0, defendedZoneTimeBeforeRefreshAggression);

            interestingTimeRemaining -= Time.deltaTime;
            if (interestingTimeRemaining <= 0)
            {
                // if cam has moved around a lot, bump the curiosity, otherwise lower it
                if (cameraMovement < cameraMovementThreshhold) CryptidBrain.Instance.curiosity += -1;
                else CryptidBrain.Instance.curiosity += 1;

                interestingTimeRemaining = interestingTime;
            }
        }
        // otherwise, reduce lose player timer
        else
        {
            //CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);

            timeRemaining -= Time.deltaTime;

            // if lost player, swap to 'hunt' mode
            if (timeRemaining <= 0)
            {
                brain.ChangeState("HuntNormal");
            }
        }

        goalTimeRemaining -= Time.deltaTime;
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

    private float measureCameraMovement()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        return mouseDelta.magnitude;
    }

    private void ToyBehaviour()
    {
        // if new goal needed, select one
        if (currentGoalZone == null || goalTimeRemaining <= 0)
        {
            currentGoalZone = chooseGoalZone();
            goalTimeRemaining = timeForSelectNewGoal;
        }

        float playerDistanceFromGoal = (CryptidBrain.Instance.senses.lastKnownPlayerLocation - currentGoalZone).magnitude;

        // if player is too far from goal, kite them
        if (playerDistanceFromGoal > maxDistanceBeforeKite)
        {
            Kite();
        }
        
    }

    private Vector3 chooseGoalZone()
    {
        // choose a goal zone close enough to where the player currently is

        List<Transform> zones = new List<Transform>(targets);

        while (zones.Count > 0)
        {
            Transform chosen = zones[Random.Range(0, zones.Count)];
            if ((chosen.position - CryptidBrain.Instance.senses.lastKnownPlayerLocation).magnitude < targetSelectionMaxDistance) return chosen.position;
            else zones.Remove(chosen);
        }
        return CryptidBrain.Instance.senses.lastKnownPlayerLocation;
    }

    private void Kite()
    {

        Vector3 kitePositionOffset = (CryptidBrain.Instance.senses.lastKnownPlayerLocation - currentGoalZone).normalized * kiteOffsetDistance;
        Vector3 kitePosition = CryptidBrain.Instance.senses.lastKnownPlayerLocation + kitePositionOffset;

        // find the position to move to and make sure it's valid
        if (FindValidLocation(kitePosition, PlayerReference.Instance.transform.position, out Vector3 destination))
            CryptidBrain.Instance.navigator.SetDestination(destination);

        else CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);
    }
}
