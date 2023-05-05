using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelPhoto : CBInteractible
{
    [SerializeField] int sceneIndex = 0;

    public override void Interact()
    {
        base.Interact();
        LoadScene.Instance.LoadNewScene(sceneIndex);
    }
}
