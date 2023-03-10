using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = PlayerReference.Instance.cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookPos = cameraTransform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
