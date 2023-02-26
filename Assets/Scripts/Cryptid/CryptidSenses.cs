using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptidSenses : MonoBehaviour
{
    // component to handle the cryptid's senses of the player

    public Vector3 lastKnownPlayerLocation;

    [Header("Sense Parameters")]
    [SerializeField] private float sightRadius = 25;
    [SerializeField] private float sightAngleInDegrees = 90;
    [SerializeField] private float sightHeightAddition = 3;
    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private float hearingRadius = 10;
    [SerializeField] private float tremorsenseRadius = 3;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask soundLayer;

    // check whether the player can be sensed by the cryptid
    public bool CanSensePlayer()
    {
        if (CheckSight() || CheckHearing() || CheckTremorSense())
        {
            lastKnownPlayerLocation = PlayerReference.Instance.transform.position;

            return true;
        }
        else return false;
    }

    private bool CheckSight() // long-range detection for player relying on line of sight
    {
        // measure from position of cryptid's eyes instead of cryptid's origin
        Vector3 sightPosition = transform.position + Vector3.up * sightHeightAddition;

        // detect if player is in sight radius
        Collider[] hitPlayer = Physics.OverlapSphere(sightPosition, sightRadius, playerLayer);
        if (hitPlayer.Length > 0) 
        {
            // player detected; check if in view angle
            Transform target = hitPlayer[0].transform;
            Vector3 directionToTarget = (target.position - sightPosition).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < sightAngleInDegrees / 2)
            {
                // in view angle, check if direct line of sight
                float distanceToTarget = Vector3.Distance(sightPosition, target.position);
                RaycastHit hit;
                if (Physics.Raycast(sightPosition, directionToTarget, out hit, distanceToTarget, sightLayerMask))
                {
                    if (hit.collider.transform == target) // if player was hit by raycast
                    {
                        Debug.DrawRay(sightPosition, directionToTarget * distanceToTarget, Color.yellow, 2f);
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
        Collider[] hitSounds = Physics.OverlapSphere(transform.position, hearingRadius, soundLayer);
        if (hitSounds.Length > 0) return true; // sounds heard in radius
        else return false;
    }

    private bool CheckTremorSense() // close-range detection for player regardless of line of sight
    {
        Collider[] hitPlayer = Physics.OverlapSphere(transform.position, tremorsenseRadius, playerLayer);
        if (hitPlayer.Length > 0) return true; // player detected in radius
        else return false;
    }

    public bool PlayerCanSeeMe()
    {
        return PolaroidCheckVisible.Instance.ICanSee(PlayerReference.Instance.cam, CryptidBrain.Instance.photographable);
    }

    void OnDrawGizmosSelected()
    {
        // draw spheres for the sense radii in the editor

        // Draw a yellow sphere for sight
        Vector3 sightPosition = transform.position + Vector3.up * sightHeightAddition;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sightPosition, sightRadius);

        // Draw a blue sphere for hearing
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        // Draw a green sphere for tremorsense
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, tremorsenseRadius);
    }
}
