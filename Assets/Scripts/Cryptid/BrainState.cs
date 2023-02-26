using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BrainState
{
    // this is a generic class to be inherited from for states for the cryptidbrain state machine

    public string name;
    protected CryptidBrain brain;
    private bool stareAtPlayer = false;

    public BrainState(string name, CryptidBrain brain)
    {
        this.name = name;
        this.brain = brain;
    }

    public virtual void Enter() { }

    public virtual void UpdateLogic()
    {
        if (stareAtPlayer)
        {
            StareAtPlayer();
        }
    }

    public virtual void Exit()
    {
        // stop pathfinding
        CryptidBrain.Instance.navigator.ResetPath();
    }

    public virtual void CryptidPhotographed() { }

    public virtual void NotCryptidPhotographed() { }

    // useful methods for a variety of states

    public virtual void EnableStareAtPlayer(bool enable)
    {
        if (enable && !stareAtPlayer)
        {
            CryptidBrain.Instance.navigator.angularSpeed = 0;
            stareAtPlayer = true;
        }
        else if (!enable && stareAtPlayer)
        {
            CryptidBrain.Instance.navigator.angularSpeed = 120;
            stareAtPlayer = false;
        }
    }

    private void StareAtPlayer()
    {
        // rotate to stare at the player, only rotating on y axis
        Transform cryptidTransform = CryptidBrain.Instance.gameObject.transform;

        Vector3 targetPostition = new Vector3(CryptidBrain.Instance.senses.lastKnownPlayerLocation.x,
                                        cryptidTransform.position.y,
                                        CryptidBrain.Instance.senses.lastKnownPlayerLocation.z);
        cryptidTransform.LookAt(targetPostition);
    }
}
