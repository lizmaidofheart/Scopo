using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monologue : MonoBehaviour
{
    // this is a singleton acting as an easy reference for the player
    // it being a singleton means its accessible from anywhere without complex getcomponents etc

    private static Monologue _instance;
    public static Monologue Instance { get { return _instance; } }

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

    [SerializeReference] public DictionaryOfStringAndString monologueText = new DictionaryOfStringAndString();

    public void PlayText()
    {

    }
}
