using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardHandler : MonoBehaviour
{
    [SerializeField] Transform playerCamTransform;
    [SerializeField] float billboardRadius = 50;
    [SerializeField] float updatesPerSecond = 10;
    [SerializeField] LayerMask billboardLayer;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateBillboardsInArea", 0, 1 / updatesPerSecond); // call billboards repeatedly
    }

    void UpdateBillboardsInArea() // get all the billboards within billboardRadius from the player, then update them
    {
        Vector3 pos = playerCamTransform.position;
        Collider[] billboardsInArea = Physics.OverlapSphere(pos, billboardRadius, billboardLayer);
        foreach (Collider bb in billboardsInArea)
        {
            bb.GetComponent<Billboard>().BillboardUpdate(pos);
        }
    }
    void OnDrawGizmosSelected()
    {
        // draw a sphere for the billboard radius
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(playerCamTransform.position, billboardRadius);
    }
}
