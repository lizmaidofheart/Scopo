using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PolaroidSnapshot : MonoBehaviour
{
    // this is a singleton for taking photos

    private static PolaroidSnapshot _instance;
    public static PolaroidSnapshot Instance { get { return _instance; } }

    // code modified from https://www.youtube.com/watch?v=d-56p770t0U

    private Camera snapshotCam;

    [SerializeField] int resWidth = 256;
    [SerializeField] int resHeight = 256;

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

        // empty existing photos folder
        DirectoryInfo photosFolder = new DirectoryInfo(@"Assets\Photos");
        FileInfo[] files = photosFolder.GetFiles();
        foreach (FileInfo f in files)
        {
            f.Delete();
        }
    }

    private void Start()
    {
        snapshotCam = PolaroidInfo.Instance.polaroidCam;

        // initialise a render texture for the cam, or set our resolution values according to the existing render texture if there already is one
        if (snapshotCam.targetTexture == null)
        {
            snapshotCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapshotCam.targetTexture.width;
            resHeight = snapshotCam.targetTexture.height;
        }
        // disable the camera
        snapshotCam.gameObject.SetActive(false);
    }

    // turns on the camera - this will only last one frame due to lateupdate
    public void CallTakeSnapshot()
    {
        snapshotCam.gameObject.SetActive(true);
    }

    // check if camera is on. if so, take a photo and record its info to the polaroidinfo singleton
    public void LateUpdate()
    {
        if (snapshotCam.gameObject.activeInHierarchy)
        {
            string fileName = TakeSnapshot();

            List<Photographable> visibleObjs = PolaroidCheckVisible.Instance.visiblePhotographables(snapshotCam);

            foreach (Photographable obj in visibleObjs)
            {
                obj.IveBeenPhotographed(); // tell a photographed object that it's been photographed.
            }

            PolaroidInfo.Instance.photos.Add(new PolaroidInfo.Photograph(fileName, visibleObjs));

            // disable cam
            snapshotCam.gameObject.SetActive(false);
        }
    }

    private string TakeSnapshot() // returns filename of snapshot
    {
        // create texture2d for photo
        Texture2D snapshot = new Texture2D(resWidth, resHeight);
        // tell camera to render
        snapshotCam.Render();
        RenderTexture.active = snapshotCam.targetTexture;
        // tell texture2d to read from rendertexture
        snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

        // turn texture2d info into png
        byte[] bytes = snapshot.EncodeToPNG();
        string fileName = SnapshotName();
        File.WriteAllBytes(fileName, bytes);
        
        return fileName;
    }

    private string SnapshotName() // naming snapshot png files
    {
        //put string in /Photos and name it with width, height and time
        return string.Format("{0}/Photos/snap_{1}x{2}_{3}.png",
            Application.dataPath,
            resWidth,
            resHeight,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
}
