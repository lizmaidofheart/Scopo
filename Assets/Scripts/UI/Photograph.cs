using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Photograph : MonoBehaviour
{
    // for handling the photograph popup in the ui

    [SerializeField] RawImage image;
    [SerializeField] Animator animator;

    public void SetPhoto(string filepath)
    {
        byte[] bytes = File.ReadAllBytes(filepath);
        Texture2D texture = new Texture2D(512,512);
        ImageConversion.LoadImage(texture, bytes);
        image.texture = texture;

        animator.SetTrigger("Pop In"); // make the photograph ui pop in then out
    }
}