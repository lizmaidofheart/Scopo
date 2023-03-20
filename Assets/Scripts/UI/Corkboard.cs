using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corkboard : PromptInArea
{
    [Header("Prompt")]
    [SerializeField] string lockedPrompt;
    [SerializeField] string unlockedPrompt;

    [Header("Camera Control")]
    [SerializeField] Transform camWaypoint;
    [SerializeField] float positionLerpRate = 0.1f;
    [SerializeField] float rotationLerpRate = 0.1f;

    [Header("References")]
    [SerializeField] FirstPersonMovement playerMovement;
    [SerializeField] FirstPersonLook playerLook;
    [SerializeField] Transform playerCam;

    Vector3 camDefaultLocalPos;
    Quaternion camDefaultLocalRot;

    bool lockedOn = false;

    public override void Start()
    {
        base.Start();
        prompt.text = unlockedPrompt;

        camDefaultLocalPos = playerCam.localPosition;
        camDefaultLocalRot = playerCam.localRotation;
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

            trackDistance = true;
        }
        else // lock the player into the corkboard
        {
            lockedOn = true;

            playerLook.enabled = false;
            playerMovement.enabled = false;
            
            prompt.text = lockedPrompt;

            Cursor.lockState = CursorLockMode.Confined;

            trackDistance = false;
        }
    }

    public override void Update()
    {
        base.Update();

        if (lockedOn) // lerp camera to waypoint position
        {
            playerCam.position = Vector3.Lerp(playerCam.position, camWaypoint.position, positionLerpRate);
            playerCam.rotation = Quaternion.Lerp(playerCam.rotation, camWaypoint.rotation, rotationLerpRate);
        }
        else // lerp camera to default position
        {
            playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, camDefaultLocalPos, positionLerpRate);
            playerCam.localRotation = Quaternion.Lerp(playerCam.localRotation, camDefaultLocalRot, rotationLerpRate);
        }
    }
}
