using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolaroidObject : MonoBehaviour
{
    [Header("Object Movement")]

    [SerializeField] Quaternion restAngle;

    [SerializeField] float restLerpRate = 0.1f;
    [SerializeField] float aimLerpRate = 0.1f;

    private LerpToAngleMatch angleMatch;
    private Transform camTransform;

    [Header("State Management")]

    [SerializeField] AimState aimState = AimState.Rest;

    [SerializeField] Flashlight flashlightReference;
    private bool setFlashlightToReenable = false;

    [Header("Camera Flash")]

    [SerializeField] Light polaroidFlash;
    [SerializeField] float flashIntensity = 50;
    [SerializeField] float flashLingerRate = 0.5f;
    [SerializeField] float flashIncreaseRate = 0.2f;

    FlashState flashState = FlashState.None;

    [Header("Input Keys")]

    [SerializeField] KeyCode aimKey;
    [SerializeField] KeyCode captureKey;

    private enum AimState
    {
        Rest,
        Aim
    }

    private enum FlashState
    {
        None,
        Increase,
        Reduce,
        JustFlashed
    }

    // Start is called before the first frame update
    void Start()
    {
        camTransform = gameObject.transform;

        camTransform.localRotation = restAngle;

        angleMatch = gameObject.GetComponent<LerpToAngleMatch>();
        angleMatch.angleToDampenTo = restAngle;
        SetState(AimState.Rest);

        polaroidFlash.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // check if aiming the camera
        if (Input.GetKeyDown(aimKey) && aimState != AimState.Aim)
        {
            SetState(AimState.Aim);
        }
        if (Input.GetKeyUp(aimKey) && aimState != AimState.Rest)
        {
            SetState(AimState.Rest);
        }

        //take photo
        if (aimState == AimState.Aim && Input.GetKeyDown(captureKey))
        {
            PolaroidSnapshot.Instance.CallTakeSnapshot();
            SetState(AimState.Rest);
        }

        if (aimState == AimState.Rest)
        {
            // slerp the x and z axis of the camera's euler rotation towards restAngle

            Vector3 camEulers = camTransform.eulerAngles;
            Vector3 restEulers = restAngle.eulerAngles;

            float lerpedX = Mathf.LerpAngle(camEulers.x, restEulers.x, restLerpRate);
            float lerpedZ = Mathf.LerpAngle(camEulers.z, restEulers.z, restLerpRate);

            camTransform.eulerAngles = new Vector3(lerpedX, camEulers.y, lerpedZ);
        }
    }

    private void LateUpdate()
    {
        // reduce lingering flash
        if (flashState == FlashState.Reduce)
        {
            polaroidFlash.intensity *= flashLingerRate;
            if (polaroidFlash.intensity <= 2f)
            {
                flashState = FlashState.None;
                polaroidFlash.intensity = 0;
            }
        }

        // increase flash while preparing
        else if (flashState == FlashState.Increase)
        {
            polaroidFlash.intensity = Mathf.Lerp(polaroidFlash.intensity, flashIntensity, flashIncreaseRate);
        }

        // if just flashed, begin to reduce flash (this is delayed so the frame when the camera takes the photo is fully lit)
        else if (flashState == FlashState.JustFlashed)
        {
            flashState = FlashState.Reduce;
        }

        if (setFlashlightToReenable)
        {
            flashlightReference.SetActive(true);
        }
    }

    private void SetState(AimState state)
    {
        aimState = state;
        if (aimState == AimState.Aim)
        {
            angleMatch.matchYOnly = false;
            angleMatch.setSlerpRate(aimLerpRate);
            flashlightReference.SetActive(false);
            flashState = FlashState.Increase;
        }
        else if (aimState == AimState.Rest)
        {
            angleMatch.matchYOnly = true;
            angleMatch.setSlerpRate(restLerpRate);
            setFlashlightToReenable = true;

            if (polaroidFlash.intensity > 0)
            {
                flashState = FlashState.JustFlashed;
            }
        }
    }
}
