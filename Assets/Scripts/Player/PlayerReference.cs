using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerReference : MonoBehaviour
{
    [SerializeField] public Camera cam;
    [SerializeField] Animator deathAnimator;
    [SerializeField] private float cluesFound = 0;
    [SerializeField] public bool hasPhotographedCryptid = false;
    [SerializeField] bool timeToDie = false;
    private List<string> clueLog = new List<string>();

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

    private void Update()
    {
        if (timeToDie)
        {
            timeToDie = false;
            Die(2);
        }
    }

    public void Die(float timeBeforeReset)
    {
        FirstPersonMovement mover = GetComponent<FirstPersonMovement>(); // stop player being able to move
        mover.speed = 0;
        deathAnimator.SetTrigger("Die");
        StartCoroutine(ResetLevel(timeBeforeReset));
    }

    private IEnumerator ResetLevel(float timeToWait) // reload scene after a specified delay
    {
        yield return new WaitForSeconds(timeToWait);
        LoadScene.Instance.LoadNewScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void FindClue(string clue)
    {
        if (!clueLog.Contains(clue)) // if clue hasnt been found yet
        {
            clueLog.Add(clue);
            cluesFound += 1;
        }
    }

    public void SetPhotographedCryptid(bool set)
    {
        hasPhotographedCryptid = set;
    }
}
