using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolaroidObject : MonoBehaviour
{
    // yes i know this script is very messy

    [Header("Object Movement")]

    [SerializeField] Quaternion restAngle;
    [SerializeField] Quaternion reloadAngle;

    [SerializeField] float restLerpRate = 0.1f;
    [SerializeField] float aimLerpRate = 0.1f;

    private LerpToAngleMatch angleMatch;
    private Transform camTransform;

    [Header("State Management")]

    [SerializeField] AimState polaroidAimState = AimState.Rest;

    [Header("Flashlight")]

    [SerializeField] Flashlight flashlightReference;
    private bool setFlashlightToReenable = false;

    [Header("Camera Flash")]

    [SerializeField] Light polaroidFlashPointLight;
    [SerializeField] ParticleSystem polaroidFlashParticle;
    [SerializeField] float flashIntensity = 50;
    [SerializeField] float flashLingerRate = 0.5f;
    [SerializeField] float flashIncreaseRate = 0.2f;

    FlashState flashState = FlashState.None;

    [Header("Input Keys")]

    [SerializeField] KeyCode aimKey;
    [SerializeField] KeyCode captureKey;

    [Header("Limiting")]

    [SerializeField] TMPro.TextMeshProUGUI filmCounterText;
    [SerializeField] int maxFilm = 10;
    [SerializeField] int currentFilm = 10;
    [SerializeField] float reloadTime = 1;
    private float remainingReloadTime = 0;

    [Header("Audio")]

    [SerializeField] AudioSource reloadSound;
    [SerializeField] AudioSource chargeSound;
    [SerializeField] AudioSource captureSound;
    [SerializeField] AudioSource noFilmLeftSound;
    [SerializeField] float chargePitchMax = 3;
    [SerializeField] float chargePitchMin = -3;
    [SerializeField] float chargePitchIncreaseRate = 1;

    private enum AimState
    {
        Rest,
        Aim,
        Reload
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

        polaroidFlashPointLight.intensity = 0;

        changeFilmCount(currentFilm);

        chargeSound.pitch = chargePitchMin;

        filmCounterText.text = currentFilm.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // check if aiming the camera. if theres remaining reloadtime, aimkey starts reloading instead
        if (Input.GetKeyDown(aimKey) && polaroidAimState != AimState.Aim && remainingReloadTime <= 0 && polaroidAimState != AimState.Reload)
        {
            if (currentFilm > 0)
            {
                SetState(AimState.Aim);
            }
            else
            {
                PlayerSoundMaker.Instance.CreateSound(noFilmLeftSound, true);
            }
            
        }
        else if (Input.GetKeyDown(aimKey) && polaroidAimState != AimState.Reload && remainingReloadTime > 0)
        {
            SetState(AimState.Reload);
        }
        else if (Input.GetKeyUp(aimKey) && polaroidAimState != AimState.Rest && polaroidAimState != AimState.Reload)
        {
            SetState(AimState.Rest);
        }

        //take photo
        if (polaroidAimState == AimState.Aim && Input.GetKeyDown(captureKey))
        {
            PolaroidSnapshot.Instance.CallTakeSnapshot();
            polaroidFlashParticle.Play();
            changeFilmCount(currentFilm - 1);

            PlayerSoundMaker.Instance.CreateSound(captureSound, true);

            if (currentFilm > 0)
            {
                SetState(AimState.Reload);
                remainingReloadTime = reloadTime;
            }
            else SetState(AimState.Rest);
        }

        // charge sound pitch handling
        if (polaroidAimState == AimState.Aim && chargeSound.pitch < chargePitchMax)
        {
            chargeSound.pitch += chargePitchIncreaseRate * Time.deltaTime;
        }
        else if (chargeSound.pitch > chargePitchMin)
        {
            chargeSound.pitch -= chargePitchIncreaseRate * Time.deltaTime;
        }

        // position slerping
        if (polaroidAimState == AimState.Rest)
        {
            slerpCamTowardsAngle(restAngle);
        }
        else if (polaroidAimState == AimState.Reload)
        {
            slerpCamTowardsAngle(reloadAngle);
        }

        // if reloading, reduce remaining reload time
        if (polaroidAimState == AimState.Reload)
        {
            remainingReloadTime -= Time.deltaTime;

            // if theres no reload time left, stop reloading
            if (remainingReloadTime <= 0)
            {
                SetState(AimState.Rest);
            }
        }
    }

    private void LateUpdate() // everything in here deals with lighting
    {
        // reduce lingering flash
        if (flashState == FlashState.Reduce)
        {
            polaroidFlashPointLight.intensity *= flashLingerRate;
            if (polaroidFlashPointLight.intensity <= 2f)
            {
                flashState = FlashState.None;
                polaroidFlashPointLight.intensity = 0;
            }
        }

        // increase flash while preparing
        else if (flashState == FlashState.Increase)
        {
            polaroidFlashPointLight.intensity = Mathf.Lerp(polaroidFlashPointLight.intensity, flashIntensity, flashIncreaseRate);
        }

        // if just flashed, begin to reduce flash (this is delayed so the frame when the camera takes the photo is fully lit)
        else if (flashState == FlashState.JustFlashed)
        {
            flashState = FlashState.Reduce;
        }

        // if flashlight is set to reenable, do so
        if (setFlashlightToReenable)
        {
            flashlightReference.SetActive(true);
            setFlashlightToReenable = false;
        }
    }

    private void SetState(AimState state) // function to swap between states
    {
        polaroidAimState = state;
        
        switch (polaroidAimState)
        {
            case AimState.Rest: // make the cam not match the players vision, turn the flashlight on and adjust flash intensity downwards

                angleMatch.matchYOnly = true;
                angleMatch.setSlerpRate(restLerpRate);
                setFlashlightToReenable = true;

                if (polaroidFlashPointLight.intensity > 0)
                {
                    flashState = FlashState.JustFlashed;
                }

                // audio
                PlayerSoundMaker.Instance.DestroyLoopingSound(reloadSound, true);
                PlayerSoundMaker.Instance.DestroyLoopingSound(chargeSound, true);

                break;

            case AimState.Aim: // make the cam match the players vision, turn the flashlight off and adjust flash intensity upwards

                angleMatch.matchYOnly = false;
                angleMatch.setSlerpRate(aimLerpRate);
                flashlightReference.SetActive(false);
                flashState = FlashState.Increase;

                // audio
                PlayerSoundMaker.Instance.CreateSound(chargeSound, true);

                break;

            case AimState.Reload: // make the cam match the players vision, turn the flashlight off, adjust flash intensity downwards

                angleMatch.matchYOnly = false;
                angleMatch.setSlerpRate(restLerpRate);
                flashlightReference.SetActive(false);

                if (polaroidFlashPointLight.intensity > 0)
                {
                    flashState = FlashState.JustFlashed;
                }

                // audio
                PlayerSoundMaker.Instance.CreateSound(reloadSound, true);
                PlayerSoundMaker.Instance.DestroyLoopingSound(chargeSound);

                break;
        }

    }

    private void changeFilmCount(int newFilmCount) // changes the film counter to a new value
    {
        currentFilm = newFilmCount;
        if (currentFilm > maxFilm)
        {
            currentFilm = maxFilm;
        }
        else if (currentFilm < 0)
        {
            currentFilm = 0;
        }
        filmCounterText.text = currentFilm.ToString();
    }

    public void refillFilmToMax() // refills the film counter to the maximum value
    {
        currentFilm = maxFilm;
        filmCounterText.text = currentFilm.ToString();
    }

    private void slerpCamTowardsAngle(Quaternion goalAngle)
    {
        // slerp the x and z axis of the camera's euler rotation towards goalAngle

        Vector3 camEulers = camTransform.eulerAngles;
        Vector3 goalEulers = goalAngle.eulerAngles;

        float lerpedX = Mathf.LerpAngle(camEulers.x, goalEulers.x, restLerpRate);
        float lerpedZ = Mathf.LerpAngle(camEulers.z, goalEulers.z, restLerpRate);

        camTransform.eulerAngles = new Vector3(lerpedX, camEulers.y, lerpedZ);
    }

}
