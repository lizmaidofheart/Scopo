using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolaroidInfo : MonoBehaviour
{
    // this is a singleton containing all the info the polaroid will need to function
    // it being a singleton means its accessible from anywhere without complex getcomponents etc

    private static PolaroidInfo _instance;
    public static PolaroidInfo Instance { get { return _instance; } }

    // info accessible from the singleton

    [SerializeField] public Camera polaroidCam;
    public List<Photograph> photos; // a public list of all photos created
    public List<Photographable> photographables; // a public list of all photographable objects

    [System.Serializable]
    public struct Photograph
    {
        public string filename;
        public List<Photographable> visibleObjs;

        public Photograph(string _filename, List<Photographable> _visibleObjs)
        {
            filename = _filename;
            visibleObjs = _visibleObjs;
        }
    }

    private void Awake() // singleton setup
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        // these being created in awake means they can be added to in other scripts' start()s
        photos = new List<Photograph>();
        photographables = new List<Photographable>();
    }
}
