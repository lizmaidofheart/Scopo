using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunt : BrainState
{
    float searchRadius = 0;

    public Hunt(string name, CryptidBrain brain) : base(name, brain) { }

    public override void Enter()
    {
        base.Enter();
        searchRadius = 0;
    }
}
