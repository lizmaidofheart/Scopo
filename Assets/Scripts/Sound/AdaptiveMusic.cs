using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveMusic : MonoBehaviour
{
    private static AdaptiveMusic _instance;
    public static AdaptiveMusic Instance { get { return _instance; } }

    // this script manages swapping between music tracks

    [SerializeReference] List<AudioSource> tracks;

    [SerializeField] float blendTime = 2;

    [SerializeField] int currentTrackIndex = 0;

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

    // Start is called before the first frame update
    void Start()
    {
        tracks[currentTrackIndex].volume = 1;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < tracks.Count; i++)
        {
            if (i == currentTrackIndex && tracks[i].volume < 1)
            {
                // blend current track's volume towards 1
                tracks[i].volume += Time.deltaTime / blendTime;
            }
            else if (tracks[i].volume > 0)
            {
                // blend all other tracks' volume towards 0
                tracks[i].volume -= Time.deltaTime / blendTime;
            }
        }
    }

    public void SwitchTrack(int newTrackIndex)
    {
        currentTrackIndex = newTrackIndex;
    }
}
