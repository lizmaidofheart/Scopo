using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : BrainState
{
    public List<Transform> targets = new List<Transform>();

    public Wander(string name, CryptidBrain brain, Transform wanderTargetsParent) : base(name, brain)
    {
        for (int i = 0; i < wanderTargetsParent.childCount; i++)
        {
            targets.Add(wanderTargetsParent.GetChild(i));
        }
    }

    public override void Enter()
    {
        base.Enter();

        // start pathfinding
        GoToNewLocation();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if at destination, pick somewhere new to pathfind to.
        if (!CryptidBrain.Instance.navigator.pathPending && CryptidBrain.Instance.navigator.remainingDistance < 0.5f)
            GoToNewLocation();

        // if player is detected, increase curiosity and swap to follow state
        if (CryptidBrain.Instance.senses.CanSensePlayer())
        {
            brain.ChangeState("Follow");
            CryptidBrain.Instance.curiosity += 1;
        }
    }

    public override void Exit()
    {
        base.Exit();

        // stop pathfinding
        CryptidBrain.Instance.navigator.ResetPath();
    }

    private void GoToNewLocation()
    {
        // pick new random location from targets
        CryptidBrain.Instance.navigator.SetDestination(targets[Random.Range(0, targets.Count)].position);
    }
}
