using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    [SerializeField] private bool isActive = true;
    [SerializeField] private Light lightBeam;

    void Start()
    {
        if (!isActive)
        {
            lightBeam.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // takes a bool, sets the spotlight to active or inactive according to that bool
    public void SetActive(bool state)
    {
        if (state && !isActive)
        {
            isActive = true;
            lightBeam.enabled = true;
        }
        else if (!state && isActive)
        {
            isActive = false;
            lightBeam.enabled = false;
        }
    }
}
