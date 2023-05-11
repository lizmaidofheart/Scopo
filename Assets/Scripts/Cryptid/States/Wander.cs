using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : BrainState
{
    public float defaultSpeed;
    public float imposedSpeed = 2.5f;
    public List<Transform> targets = new List<Transform>();

    public Wander(string name, CryptidBrain brain, Transform wanderTargetsParent, float newSpeed) : base(name, brain)
    {
        for (int i = 0; i < wanderTargetsParent.childCount; i++)
        {
            targets.Add(wanderTargetsParent.GetChild(i));
        }

        imposedSpeed = newSpeed;
    }

    public override void Enter()
    {
        base.Enter();

        defaultSpeed = CryptidBrain.Instance.navigator.speed;
        CryptidBrain.Instance.navigator.speed = imposedSpeed;

        // start pathfinding
        GoToNewLocation();

        AdaptiveMusic.Instance.SwitchTrack(0);
        CryptidBrain.Instance.animator.SetBool("isWalking", true);

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // if at destination, pick somewhere new to pathfind to.
        if (!CryptidBrain.Instance.navigator.pathPending && CryptidBrain.Instance.navigator.remainingDistance < 0.5f) GoToNewLocation();

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

        CryptidBrain.Instance.navigator.speed = defaultSpeed;
    }

    private void GoToNewLocation()
    {
        // pick new random location from targets
        CryptidBrain.Instance.navigator.SetDestination(targets[Random.Range(0, targets.Count)].position);
    }
}
