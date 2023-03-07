using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Monologue : MonoBehaviour
{
    // this is a singleton acting as an easy reference for the monologue text
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

    // this script manages the monologue text at the top of the screen

    private TextMeshProUGUI text;
    [SerializeField] float timeBetweenCharacters = 0.05f;
    [SerializeField] float timeToStay = 3;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        PlayText();
    }

    public void PlayText()
    {
        StartCoroutine(TextTypewriter());
    }

    private IEnumerator TextTypewriter() // make text appear one character by one, then 
    {
        int max = text.text.Length;
        text.maxVisibleCharacters = 0;

        while (text.maxVisibleCharacters < max)
        {
            text.maxVisibleCharacters += 1;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        yield return new WaitForSeconds(timeToStay);

        text.maxVisibleCharacters = 0;
    }
}
