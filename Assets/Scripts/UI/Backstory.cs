using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Backstory : MonoBehaviour
{
    [SerializeField] List<string> lines = new List<string>();
    [SerializeField] KeyCode startKey;
    [SerializeField] float timeBetweenCharacters = 0.05f;
    [SerializeField] float timeToStay = 3;
    [SerializeField] float timeBetweenLines = 0.5f;

    private TextMeshProUGUI textUI;
    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(startKey) && !activated)
        {
            activated = true;
            StartCoroutine(PlayBackstory());
        }
    }

    IEnumerator PlayBackstory() // play the lines one after another
    {
        for (int i = 0; i < lines.Count; i++)
        {
            textUI.text = lines[i];
            yield return StartCoroutine(TextTypewriter());
            yield return new WaitForSeconds(timeBetweenLines);
        }
    }

    IEnumerator TextTypewriter() // make text appear one character by one, then stay
                                 // for the specified time before being removed
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
