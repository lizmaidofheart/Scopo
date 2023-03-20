using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corkboard : PromptInArea
{
    [SerializeField] string lockedPrompt;
    [SerializeField] string unlockedPrompt;

    [SerializeField] Vector3 lockedPosition;
    [SerializeField] Vector3 lookOffset;
    [SerializeField] float positionLerpRate = 0.1f;
    [SerializeField] float rotationLerpRate = 0.1f;

    [SerializeField] FirstPersonMovement playerMovement;
    [SerializeField] FirstPersonLook playerLook;

    [SerializeField] Camera corkboardCamera;
    [SerializeField] Camera playerCamera;

    bool lockedOn = false;

    public override void Start()
    {
        base.Start();
        prompt.text = unlockedPrompt;
        corkboardCamera.enabled = false;
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

            Cursor.lockState = CursorLockMode.Locked;

            // swap cameras
            corkboardCamera.enabled = false;
            playerCamera.enabled = true;
        }
        else // lock the player into the corkboard
        {
            lockedOn = true;

            playerLook.enabled = false;
            playerMovement.enabled = false;
            
            prompt.text = lockedPrompt;

            Cursor.lockState = CursorLockMode.Confined;

            // swap cameras
            corkboardCamera.enabled = true;
            playerCamera.enabled = false;
        }
    }

    public override void Update()
    {
        base.Update();
    }
}
