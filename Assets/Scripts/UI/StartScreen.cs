using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartScreen : MonoBehaviour
{
    [SerializeField] FirstPersonLook playerLook;
    [SerializeField] FirstPersonMovement playerMovement;

    [SerializeField] List<TextMeshProUGUI> textElements;

    [SerializeField] KeyCode startKey;

    [SerializeField] float textFadeTime = 1;

    bool fadeAway = false;
    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement.enabled = false;
        playerLook.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(startKey) && !activated)
        {
            fadeAway = true;
            activated = true;

            playerMovement.enabled = true;
            playerLook.enabled = true;
        }

        if (fadeAway) // fade text
        {
            foreach(TextMeshProUGUI text in textElements)
            {
                Color textColour = text.color;
                textColour.a -= Time.deltaTime / textFadeTime;
                text.color = textColour;
                if (textColour.a <= 0) fadeAway = false;
            }
        }
    }
}
