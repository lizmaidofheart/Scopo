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

    [Header("UI Interaction")]
    [SerializeField] LayerMask raycastMask;
    [SerializeField] KeyCode interactKey;

    [Header("References")]
    [SerializeField] FirstPersonMovement playerMovement;
    [SerializeField] FirstPersonLook playerLook;
    [SerializeField] Rigidbody playerBody;
    [SerializeField] Transform playerCam;

    Vector3 camDefaultLocalPos;
    Quaternion camDefaultLocalRot;

    bool lockedOn = false;

    CBInteractible highlightedInteractible;

    public override void Start()
    {
        base.Start();
        prompt.text = unlockedPrompt;

        camDefaultLocalPos = playerCam.localPosition;
        camDefaultLocalRot = playerCam.localRotation;
    }

    public override void Update()
    {
        base.Update();

        LerpCamToWaypoint();

        if (lockedOn) // UI interaction handling
        {
            WhatAmIPointingAt();

            if (Input.GetKeyDown(interactKey) && highlightedInteractible != null)
            {
                highlightedInteractible.Interact();
            }
        }
        
    }

    public override void Action() // on press space
    {
        base.Action();
        if (!lockedOn) // lock the player into the corkboard
        {
            lockedOn = true;

            playerLook.enabled = false;
            playerMovement.enabled = false;
            playerBody.constraints = RigidbodyConstraints.FreezeRotation;

            prompt.text = lockedPrompt;

            Cursor.lockState = CursorLockMode.Confined;

            trackDistance = false;
        }
        else // unlock the player from the corkboard 
        {
            lockedOn = false;

            playerLook.enabled = true;
            playerMovement.enabled = true;
            playerBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            prompt.text = unlockedPrompt;

            Cursor.lockState = CursorLockMode.Locked;

            trackDistance = true;
        }
    }

    private void LerpCamToWaypoint()
    {
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

    private void WhatAmIPointingAt()
    {
        // cast raycast to mouse world position

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, layerMask: raycastMask))
        {
            Debug.DrawRay(ray.origin, hit.point - ray.origin);
            Debug.Log(hit.transform.name);

            // if raycast hits a CBInteractible, store it. else, null the stored one

            CBInteractible hitInteractible = hit.transform.gameObject.GetComponent<CBInteractible>();
            if (hitInteractible != null)
            {
                highlightedInteractible = hitInteractible;
                highlightedInteractible.SwitchOutline(true);
            }
            else
            {
                highlightedInteractible.SwitchOutline(false);
                highlightedInteractible = null;

            }
        }
    }
}
