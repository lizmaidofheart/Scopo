using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptidSenses : MonoBehaviour
{
    // component to handle the cryptid's senses of the player

    [Header("Sense Parameters")]
    [SerializeField] private float sightRadius = 25;
    [SerializeField] private float sightAngleInDegrees = 90;
    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private float hearingRadius = 10;
    [SerializeField] private float tremorsenseRadius = 3;

    [Header("Collision Layers")]
    [SerializeField] private int playerLayer = 9;
    [SerializeField] private int soundLayer = 8;

    // check whether the player can be sensed by the cryptid
    public bool CanSensePlayer()
    {
        if (CheckSight() || CheckHearing() || CheckTremorSense())
        {
            return true;
        }
        else return false;
    }

    private bool CheckSight() // long-range detection for player relying on line of sight
    {
        // detect if player is in sight radius
        Collider[] hitPlayer = Physics.OverlapSphere(transform.position, sightRadius, 1 << playerLayer);
        if (hitPlayer.Length > 0) 
        {
            // player detected; check if in view angle
            Transform target = hitPlayer[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < sightAngleInDegrees / 2)
            {
                // in view angle, check if direct line of sight
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, sightLayerMask))
                {
                    if (hit.collider.transform == target) // if player was hit by raycast
                    {
                        Debug.DrawRay(transform.position, directionToTarget * distanceToTarget, Color.yellow, 2f);
                        return true; // player is visible!
                    }
                    //else, player in view angle but blocked by obstacle
                }
            }
            //else, player in sight radius but not in view angle
        }
        //else, player not detected at all"
        return false;
    }

    private bool CheckHearing() // mid-range detection for sounds made by player regardless of line of sight
    {
        Collider[] hitSounds = Physics.OverlapSphere(transform.position, hearingRadius, 1 << soundLayer);
        if (hitSounds.Length > 0) // sounds heard in radius
        {
            return true;
        }
        else return false;
    }

    private bool CheckTremorSense() // close-range detection for player regardless of line of sight
    {
        Collider[] hitPlayer = Physics.OverlapSphere(transform.position, hearingRadius, 1 << playerLayer);
        if (hitPlayer.Length > 0) // player detected
        {
            return true;
        }
        else return false;
    }

    void OnDrawGizmosSelected()
    {
        // draw spheres for the sense radii

        // Draw a yellow sphere for sight
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        // Draw a blue sphere for hearing
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        // Draw a green sphere for tremorsense
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, tremorsenseRadius);
    }
}
