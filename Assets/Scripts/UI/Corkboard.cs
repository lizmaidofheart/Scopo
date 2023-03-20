using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corkboard : PromptInArea
{
    [SerializeField] string lockedPrompt;
    [SerializeField] string unlockedPrompt;

    [SerializeField] Vector3 lockedPosition;
    [SerializeField] float lockedBodyRotationX;
    [SerializeField] float lockedLookRotationY;
    [SerializeField] float positionLerpRate = 0.1f;
    [SerializeField] float rotationLerpRate = 0.1f;

    [SerializeField] FirstPersonMovement playerMovement;
    [SerializeField] FirstPersonLook playerLook;

    bool lockedOn = false;

    public override void Start()
    {
        base.Start();
        prompt.text = unlockedPrompt;
    }

    public override void Action() // on press space
    {
        base.Action();
        if (lockedOn) // unlock the player from the corkboard
        {
            lockedOn = false;

            playerLook.enabled = true;
            playerMovement.enabled = true;

            prompt.text = unlockedPrompt;
        }
        else // lock the player into the corkboard
        {
            lockedOn = true;
            playerLook.enabled = false;
            playerMovement.enabled = false;
            
            prompt.text = lockedPrompt;
        }
    }

    public override void Update()
    {
        base.Update();
        if (lockedOn)
        {
            playerTransform.position = Vector3.Lerp(playerTransform.position, lockedPosition, positionLerpRate);
            playerTransform.eulerAngles = new Vector3(Mathf.Lerp(playerTransform.eulerAngles.x, lockedBodyRotationX, rotationLerpRate), 0, 0);
            playerLook.transform.eulerAngles = new Vector3(0, Mathf.Lerp(playerLook.transform.eulerAngles.y, lockedLookRotationY, rotationLerpRate), 0);
        }
    }
}
