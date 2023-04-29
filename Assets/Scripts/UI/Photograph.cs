using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Photograph : MonoBehaviour
{
    [SerializeField] RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPhoto(string filepath)
    {
        byte[] bytes = File.ReadAllBytes(filepath);
        Texture2D texture = new Texture2D(512,512);
        ImageConversion.LoadImage(texture, bytes);
        image.texture = texture;
    }
}
