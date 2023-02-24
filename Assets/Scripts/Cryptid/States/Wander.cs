using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : BrainState
{
    public List<Transform> targets = new List<Transform>();

    public Wander(CryptidBrain brain, Transform wanderTargetsParent) : base("Wander", brain)
    {
        for (int i = 0; i < wanderTargetsParent.childCount; i++)
        {
            targets.Add(wanderTargetsParent.GetChild(i));
        }
    }
}
