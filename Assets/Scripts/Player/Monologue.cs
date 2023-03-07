using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Monologue : MonoBehaviour
{
    // this script manages the monologue text at the top of the screen

    [Header("Text Input")]
    [SerializeField] List<string> textKeys;
    [SerializeField] List<string> textValues;
    private Dictionary<string, string> monologueText = new Dictionary<string, string>();

    [Header("UI")]
    private TextMeshProUGUI textUI;
    [SerializeField] float timeBetweenCharacters = 0.05f;
    [SerializeField] float timeToStay = 3;

    private void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();

        // create monologueText
        for (int i = 0; i < textKeys.Count; i++)
        {
            monologueText[textKeys[i]] = textValues[i];
        }
    }

    public void PlayText(string key) // play monologue text for the given key
    {
        if (monologueText.ContainsKey(key))
        {
            textUI.text = monologueText[key];
            StartCoroutine(TextTypewriter());
        }
        
    }

    private IEnumerator TextTypewriter() // make text appear one character by one, then 
    {
        int max = textUI.text.Length;
        textUI.maxVisibleCharacters = 0;

        while (textUI.maxVisibleCharacters < max)
        {
            textUI.maxVisibleCharacters += 1;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        yield return new WaitForSeconds(timeToStay);

        textUI.maxVisibleCharacters = 0;
    }
}
