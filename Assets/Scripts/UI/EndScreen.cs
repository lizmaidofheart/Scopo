using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{

    [SerializeField] Photograph image1;
    [SerializeField] Photograph image2;
    [SerializeField] Photograph image3;
    [SerializeField] KeyCode quitKey;
    [SerializeField] private List<PolaroidInfo.PhotoData> sortedPhotos;

    // Start is called before the first frame update
    void Start()
    {
        // sort list of photos into best to worst
        sortedPhotos = SortPhotos(PolaroidInfo.Instance.photos);

        // put sorted photos on screen. if list is too small, delete photos from screen

        image1.SetPhotoWithoutAnimation(sortedPhotos[0].filename);

        if (sortedPhotos.Count > 1)
        {
            image2.SetPhotoWithoutAnimation(sortedPhotos[1].filename);
        }
        else
        {
            Destroy(image2.gameObject);
        }

        if (sortedPhotos.Count > 2)
        {
            image3.SetPhotoWithoutAnimation(sortedPhotos[2].filename);
        }
        else
        {
            Destroy(image3.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(quitKey))
        {
            Application.Quit();
        }
    }

    List<PolaroidInfo.PhotoData> SortPhotos(List<PolaroidInfo.PhotoData> input)
    {
        // code from https://dotnetcoretutorials.com/basic-sorting-algorithms-in-c/
        var clonedList = new List<PolaroidInfo.PhotoData>(input.Count);

        for (int i = 0; i < input.Count; i++)
        {
            var item = input[i];
            var currentIndex = i;

            while (currentIndex > 0 && FirstPhotoIsBetter(item, clonedList[currentIndex - 1]))
            {
                currentIndex--;
            }

            clonedList.Insert(currentIndex, item);
        }

        return clonedList;
    }

    bool FirstPhotoIsBetter(PolaroidInfo.PhotoData photo1, PolaroidInfo.PhotoData photo2)
    {
        // if one contains cryptid and the other doesnt, return the one with the cryptid
        if (photo1.containsCryptid && !photo2.containsCryptid) { return true;  }
        else if (!photo1.containsCryptid && photo2.containsCryptid) { return false; }

        // if both contain cryptid, return the one with the closer distance to the cryptid
        else if (photo1.containsCryptid && photo2.containsCryptid)
        {
            if (photo1.distanceToCryptid < photo2.distanceToCryptid) { return true; }
            else { return false;  }
        }
        
        else
        {
            // if neither contain cryptid, return the one with more clues photographed

            if (photo1.amountOfClues > photo2.amountOfClues) { return true; }
            else if (photo2.amountOfClues > photo1.amountOfClues) { return false; }

            // if both have same amount of clues photographed, return the one with the closest clue
            else
            {
                if (photo1.closestDistanceToClue < photo2.closestDistanceToClue) { return true; }
                else { return false; }
            }
        }
    }
}
