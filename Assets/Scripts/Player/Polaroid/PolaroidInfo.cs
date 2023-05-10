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

    public List<PhotoData> photos; // a public list of all photos created
    public List<Photographable> photographables; // a public list of all photographable objects

    [System.Serializable]
    public struct PhotoData
    {
        public string filename;
        public Dictionary<Photographable, float> visibleObjsWithDistances;
        public bool containsCryptid;
        public float distanceToCryptid;
        public float closestDistanceToClue;
        public int amountOfClues;

        public PhotoData(string _filename, Dictionary<Photographable, float> _visibleObjs)
        {
            filename = _filename;
            visibleObjsWithDistances = _visibleObjs;

            containsCryptid = false;
            distanceToCryptid = -1;
            closestDistanceToClue = 10000000;
            amountOfClues = 0;

            GetComparisonData();
        }

        private void GetComparisonData()
        {
            foreach (KeyValuePair<Photographable, float> entry in visibleObjsWithDistances)
            {
                if (entry.Key.identity == "???")
                {
                    containsCryptid = true;
                    distanceToCryptid = entry.Value;
                }
                else if (entry.Key.isClue)
                {
                    if (closestDistanceToClue > entry.Value) { closestDistanceToClue = entry.Value; }
                    amountOfClues += 1;
                }
            }
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
        photos = new List<PhotoData>();
        photographables = new List<Photographable>();

        DontDestroyOnLoad(gameObject);
    }
}
