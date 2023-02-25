using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    // this is a singleton acting as an easy reference for the player
    // it being a singleton means its accessible from anywhere without complex getcomponents etc

    private static PlayerReference _instance;
    public static PlayerReference Instance { get { return _instance; } }

    private void Awake()
    {
        // singleton setup

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
