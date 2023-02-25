using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    // if non-looping, exists for as long as its told to, then deletes itself.
    // if looping, playersoundmaker handles its deletion

    public bool looping = false;
    public float duration = 1;

    private void Update()
    {
        if (!looping)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
