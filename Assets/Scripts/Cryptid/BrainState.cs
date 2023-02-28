using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class BrainState
{
    // this is a generic class to be inherited from for states for the cryptidbrain state machine

    public string name;
    protected CryptidBrain brain;
    private bool stareAtPlayer = false;
    private bool playerInDefendedZone;
    private float defenceTimer;

    public BrainState(string name, CryptidBrain brain)
    {
        this.name = name;
        this.brain = brain;
    }

    public virtual void Enter()
    {
        playerInDefendedZone = false;
    }

    public virtual void UpdateLogic()
    {
        // manage staring
        if (stareAtPlayer) StareAtPlayer();
    }

    public virtual void Exit()
    {
        // stop pathfinding
        CryptidBrain.Instance.navigator.ResetPath();
    }

    public virtual void CryptidPhotographed() { }

    public virtual void NotCryptidPhotographed() { }


    // useful methods for a variety of states

    public void EnableStareAtPlayer(bool enable)
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

    public void DefendedZoneHandling(float aggressionOnEnter, float aggressionOnStay, float aggressionOnLeave, float timerForAggressionIncrease)
    {
        bool nearDefended = CryptidBrain.Instance.PlayerNearDefendedZone();

        if (nearDefended && !playerInDefendedZone) // entered zone
        {
            playerInDefendedZone = true;
            CryptidBrain.Instance.aggression += aggressionOnEnter;
            if (timerForAggressionIncrease != -1)
            {
                defenceTimer = timerForAggressionIncrease;
            }
        }

        else if (!nearDefended && playerInDefendedZone) // left zone
        {
            playerInDefendedZone = false;
            CryptidBrain.Instance.aggression += aggressionOnLeave;
        }

        else if (nearDefended && playerInDefendedZone) // stayed in zone
        {
            if (timerForAggressionIncrease != -1)
            {
                defenceTimer -= Time.deltaTime;
                if (defenceTimer <= 0)
                {
                    CryptidBrain.Instance.aggression += aggressionOnStay;
                    defenceTimer = timerForAggressionIncrease;
                }
            }
        }
    }

    public bool FindValidLocation(Vector3 ideal, Vector3 centre, out Vector3 result)
    {
        Vector3 skyLocation = ideal + Vector3.up * 50;

        for (int i = 0; i < 5; i++)
        {
            // on second attempt and onwards, increase range by 5
            if (i != 0)
            {
                skyLocation = skyLocation + (skyLocation - centre).normalized * 5;
            }

            if (Physics.Raycast(skyLocation, Vector3.down, out RaycastHit hit, 100, CryptidBrain.Instance.groundLayer))
            {
                //Debug.DrawRay(skyLocation, Vector3.down * 100, Color.white, 5, true);

                NavMeshPath path = new NavMeshPath();
                if (CryptidBrain.Instance.navigator.CalculatePath(hit.point, path))
                {
                    result = hit.point;
                    return true;
                }
            }
        }

        result = Vector3.zero;
        return false;
    }    
}
