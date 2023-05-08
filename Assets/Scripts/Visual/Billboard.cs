using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    // Called by handler when billboard is required to update
    public void BillboardUpdate(Vector3 goalPosition)
    {
        Vector3 lookPos = goalPosition - transform.position;
        lookPos.y = 0; // means that only yaw is changed
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
