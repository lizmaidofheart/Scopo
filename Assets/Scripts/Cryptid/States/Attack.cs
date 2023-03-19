using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BrainState
{
    public float duration = 2;

    public Attack(string name, CryptidBrain brain, float animationDuration) : base(name, brain)
    {
        duration = animationDuration;
    }

    public override void Enter()
    {
        base.Enter();
        CryptidBrain.Instance.playerObstacle.radius = 1;
        // play attack animation
        PlayerReference.Instance.Die(duration + 1);

        AdaptiveMusic.Instance.SwitchTrack(-1); // silence all music
    }

}
