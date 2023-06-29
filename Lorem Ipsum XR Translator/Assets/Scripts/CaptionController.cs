using ObjectDetection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CaptionController : MonoBehaviour
{
    public GameObject CaptionPrefab;

    List<GameObject> CaptionList;

    // Start is called before the first frame update
    void Start()
    {
        CaptionList = new List<GameObject>();    
    }

    /// <summary>
    /// Deletes all current captions in world and clears the list
    /// </summary>
    public void ClearCaptions()
    {
        // Pooling our caption objects might be more performant?
        foreach (var c in CaptionList)
        {
            Destroy(c);
        }
        CaptionList.Clear();
    }

    /// <summary>
    /// Creates a new caption object and places it in the world facing towards the player
    /// </summary>
    /// <param name="captionName">The text to be displayed on the caption</param>
    /// <param name="captionLocation">The Vector location of where the caption should be placed</param>
    public void CreateCaption(string captionName, Vector3 captionLocation)
    {
        GameObject caption = Instantiate(CaptionPrefab, captionLocation, Quaternion.identity);
        caption.transform.Find("CaptionText").GetComponent<TextMeshPro>().text = captionName;
        caption.transform.rotation.SetLookRotation(Camera.main.transform.position, Vector3.up);

        CaptionList.Add(caption);
    }
}
