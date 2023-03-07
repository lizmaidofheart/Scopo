using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerReference : MonoBehaviour
{
    [SerializeField] public Camera cam;

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

    public void Die(float timeBeforeReset)
    {
        FirstPersonMovement mover = GetComponent<FirstPersonMovement>(); // stop player being able to move
        mover.speed = 0;
        StartCoroutine(ResetLevel(timeBeforeReset));
    }

    private IEnumerator ResetLevel(float timeToWait) // reload scene after a specified delay
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
