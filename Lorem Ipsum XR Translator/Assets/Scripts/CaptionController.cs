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

    public void ClearCaptions()
    {
        // Pooling our caption objects might be more performant?
        foreach (var c in CaptionList)
        {
            Destroy(c);
        }
        CaptionList.Clear();
    }

    public void CreateCaption(string captionName, Vector3 captionLocation)
    {
        GameObject caption = Instantiate(CaptionPrefab, captionLocation, Quaternion.identity);
        caption.transform.Find("CaptionText").GetComponent<TextMeshPro>().text = captionName;
        caption.transform.rotation.SetLookRotation(Camera.main.transform.position, Vector3.up);

        CaptionList.Add(caption);
    }
}
