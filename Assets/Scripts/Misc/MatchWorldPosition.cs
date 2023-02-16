using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchWorldPosition : MonoBehaviour
{
    [SerializeField] Transform objectPositionToMatch;
    private Transform myPosition;
    [SerializeField] float objectHeight;
    private Vector3 additiveHeight;
    [SerializeField] public bool isActive = true;

    // Start is called before the first frame update
    void Start()
    {
        myPosition = gameObject.transform;
        additiveHeight = new Vector3(0, objectHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            myPosition.position = objectPositionToMatch.position + additiveHeight;
        }
        
    }
}
