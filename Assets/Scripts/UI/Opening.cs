using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Opening : MonoBehaviour
{
    private TextMeshProUGUI textUI;

    [SerializeField] private float delayTime = 6;
    [SerializeField] private float textFadeTime = 1;

    private bool fadeAway = false;

    // Start is called before the first frame update
    void Start()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        StartCoroutine(delayThenFade());
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeAway) // fade text
        {
            Color textColour = textUI.color;
            textColour.a -= Time.deltaTime / textFadeTime;
            textUI.color = textColour;
            if (textColour.a <= 0) fadeAway = false;
        }
    }

    IEnumerator delayThenFade()
    {
        yield return new WaitForSeconds(delayTime);
        fadeAway = true;
    }
}
