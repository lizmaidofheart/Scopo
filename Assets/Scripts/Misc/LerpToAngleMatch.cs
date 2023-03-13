using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToAngleMatch : MonoBehaviour
{
    // this component gets the associated object to lerp to match the rotation of a target transform.
    // it can either match on all axis, or just the y axis (the yaw) while dampening the match on the other axis

    [SerializeField] Transform match;
    [SerializeField] float slerpRate = 0.1f;
    [SerializeField] float slerpDampener = 0.5f;
    [SerializeField] Quaternion angleOffset;
    [SerializeField] Quaternion dampenAngleOffset;
    Transform myTransform;
    [SerializeField] public bool isActive = true;
    [SerializeField] public bool matchYOnly = false;

    public Quaternion angleToDampenTo;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Quaternion slerped = Quaternion.Slerp(myTransform.rotation, match.rotation * angleOffset, slerpRate * Time.deltaTime);

            if (matchYOnly) // mostly only move on the y axis
            {
                // this follows the yaw normally, but slerps towards a dampened version of the match angle instead for pitch and roll

                Vector3 eulerToDampenTo = angleToDampenTo.eulerAngles;
                eulerToDampenTo.y = slerped.eulerAngles.y;

                Quaternion modifiedAngleToDampenTo = Quaternion.Euler(eulerToDampenTo.x, eulerToDampenTo.y, eulerToDampenTo.z);

                Quaternion dampenedMatch = Quaternion.Lerp(modifiedAngleToDampenTo * dampenAngleOffset, match.rotation * angleOffset, slerpDampener);
                Quaternion reslerped = Quaternion.Slerp(myTransform.rotation, dampenedMatch, 0.2f);

                Vector3 modifiedRotation = myTransform.eulerAngles;
                modifiedRotation.y = slerped.eulerAngles.y;

                modifiedRotation.x = reslerped.eulerAngles.x;
                modifiedRotation.z = reslerped.eulerAngles.z;

                myTransform.eulerAngles = modifiedRotation;
            }
            else // move on all axis
            {
                myTransform.rotation = slerped;
            }
        }
    }

    public void setAngleOffset(Quaternion offset)
    {
        angleOffset = offset;
    }

    public void setSlerpRate(float rate)
    {
        slerpRate = rate;
    }
}
