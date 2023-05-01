using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Leave : PromptInArea
{
    [SerializeField] int sceneIndex = 2;
    string defaultPrompt;
    [SerializeField] string rejectionPrompt;
    [SerializeField] float rejectionTime = 5;
    float rejectionTimer = 5;
    bool rejectionTimerActive = false;

    public override void Start()
    {
        base.Start();

        defaultPrompt = prompt.text;
    }

    public override void Update()
    {
        base.Update();

        if (rejectionTimerActive)
        {
            rejectionTimer -= Time.deltaTime;
            if (rejectionTimer <= 0)
            {
                prompt.text = defaultPrompt;
                rejectionTimerActive = false;
            }
        }
    }

    public override void Action()
    {
        base.Action();

        if (PlayerReference.Instance.hasPhotographedCryptid) // only let player leave if theyve photographed the cryptid
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else // otherwise, change prompt to a rejection thatll change back after a timer runs out
        {
            prompt.text = rejectionPrompt;
            rejectionTimer = rejectionTime;
            rejectionTimerActive = true;
        }
    }
}
