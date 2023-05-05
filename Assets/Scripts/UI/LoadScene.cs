using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // this is a singleton

    private static LoadScene _instance;
    public static LoadScene Instance { get { return _instance; } }

    private void Awake()
    {
        // singleton setup
        _instance = this;
    }

    [SerializeField] GameObject screen;
    [SerializeField] Canvas canvas;

    private bool loadingScene = false;

    public void LoadNewScene(int index)
    {
        if (!loadingScene) // check that a scene isnt already being loaded
        {
            loadingScene = true;

            Instantiate(screen, canvas.transform); // create the loadscreen

            StartCoroutine(Loader(index));


        }
        
    }

    IEnumerator Loader(int index)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(index);

        while (!async.isDone) // wait til the scene load is done
        {
            yield return null;
        }
    }
}
