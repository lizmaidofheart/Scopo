using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundMaker : MonoBehaviour
{
    // this is a singleton handling all the sounds the player makes so that the cryptid can sense them
    // it being a singleton means its accessible from anywhere without complex getcomponents etc

    private static PlayerSoundMaker _instance;
    public static PlayerSoundMaker Instance { get { return _instance; } }

    Dictionary<AudioSource, Object> loopingSounds = new Dictionary<AudioSource, Object>();

    [SerializeField] GameObject soundPrefab;
    [SerializeField] Transform player;

    private void Awake()
    {
        // singleton setup

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // plays a sound, and creates an object to be detected. if parented, it'll stick to the player.
    // if the sound is looping, it'll need another input to destroy. otherwise, it'll last as long as the sound clip's duration.
    public void CreateSound(AudioSource sound, bool parented)
    {
        sound.Play();

        if (sound.loop)
        {
            GameObject instance;

            if (parented) instance = Instantiate(soundPrefab, player);
            else instance = Instantiate(soundPrefab, player.position, Quaternion.identity);

            instance.GetComponent<PlayerSound>().looping = true;
            loopingSounds[sound] = instance;
        }
        else
        {
            GameObject instance;

            if (parented) instance = Instantiate(soundPrefab, player);
            else instance = Instantiate(soundPrefab, player.position, Quaternion.identity);

            instance.GetComponent<PlayerSound>().duration = sound.clip.length;
        }
    }

    // disables a looping sound and destroys the matching object
    public void DestroyLoopingSound(AudioSource sound, bool ignoreError = false)
    {
        if (loopingSounds.ContainsKey(sound))
        {
            sound.Stop();

            Destroy(loopingSounds[sound]);
            loopingSounds.Remove(sound);
        }
        else if (!ignoreError) Debug.Log("PlayerSoundMaker: sound was requested but did not exist in dictionary: " + sound.ToString()); // bug reporter
    }
}
