using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hunt : BrainState
{
    public float searchRadius = 0;
    public float searchRadiusIncrease = 5;
    public float reduceAggressionOnFailure = 1;
    public float chaseAggressionThreshold = 20;
    public float toyCuriosityThreshold = 6;
    public bool lurkOnRediscovery = false;
    public float timeToStopCaring = 20;
    public float timeRemaining;
    public string stoppedCaringState = "Wander";

    public Hunt(string name, CryptidBrain brain, float newSearchRadiusIncrease, float newReduceAggressionOnFailure,
        float chase, float toy, bool lurk, float time, string stopState) : base(name, brain)
    {
        searchRadiusIncrease = newSearchRadiusIncrease;
        reduceAggressionOnFailure = newReduceAggressionOnFailure;
        chaseAggressionThreshold = chase;
        toyCuriosityThreshold = toy;
        lurkOnRediscovery = lurk;
        timeToStopCaring = time;
        stoppedCaringState = stopState;
    }

    public override void Enter()
    {
        base.Enter();
        searchRadius = 0;
        timeRemaining = timeToStopCaring;
        CryptidBrain.Instance.navigator.SetDestination(CryptidBrain.Instance.senses.lastKnownPlayerLocation);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if at destination, increase search radius, decrease aggression and pick somewhere new to pathfind to.
        if (!CryptidBrain.Instance.navigator.pathPending && CryptidBrain.Instance.navigator.remainingDistance < 0.5f)
        {
            CryptidBrain.Instance.aggression -= reduceAggressionOnFailure;
            searchRadius += searchRadiusIncrease;

            NewSearchLocation();
        }

        // if player is detected, swap state
        if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            if (CryptidBrain.Instance.aggression >= chaseAggressionThreshold && chaseAggressionThreshold != -1) brain.ChangeState("Chase");
            else if (CryptidBrain.Instance.curiosity >= toyCuriosityThreshold && toyCuriosityThreshold != -1) brain.ChangeState("Toy");
            else if (lurkOnRediscovery) brain.ChangeState("Lurk");
            else
            {
                CryptidBrain.Instance.curiosity += 1;
                brain.ChangeState("Follow");
            }

        }

        // timer count down

        timeRemaining -= Time.deltaTime;

        // if its been too long since the player's been detected, swap state
        if (timeRemaining <= 0)
        {
            CryptidBrain.Instance.aggression -= reduceAggressionOnFailure;
            brain.ChangeState(stoppedCaringState);
        }
    }

    private void NewSearchLocation()
    {
        NavMeshPath path = new NavMeshPath();

        while (path.status != NavMeshPathStatus.PathComplete)
        {
            //  pick a random location in the sky on the edge of a circle with radius searchradius and centre from the last known player location
            Vector2 searchLocationOffset2D = Random.insideUnitCircle.normalized * searchRadius;
            Vector3 searchLocationOffset3D = new Vector3(searchLocationOffset2D.x, 0, searchLocationOffset2D.y);
            Vector3 searchLocationSky = CryptidBrain.Instance.senses.lastKnownPlayerLocation + searchLocationOffset3D + Vector3.up * 50;

            // raycast down from the chosen location in the sky to find the equivalent location on the ground

            if (Physics.Raycast(searchLocationSky, Vector3.down, out RaycastHit searchLocation, 100, CryptidBrain.Instance.groundLayer))
            {
                Debug.DrawRay(searchLocationSky, Vector3.down * 100, Color.white, 5, true);

                // create a path to the chosen location and make sure it's valid
                if (!CryptidBrain.Instance.navigator.CalculatePath(searchLocation.point, path))
                {
                    continue;
                }
            }

            // if not valid, try again
        }
        
        // set the path
        CryptidBrain.Instance.navigator.SetPath(path);

    }

}
