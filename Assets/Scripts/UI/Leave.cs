using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Leave : PromptInArea
{
    [SerializeField] int sceneIndex = 2;

    public override void Action()
    {
        base.Action();
        SceneManager.LoadScene(sceneIndex);
    }
}
