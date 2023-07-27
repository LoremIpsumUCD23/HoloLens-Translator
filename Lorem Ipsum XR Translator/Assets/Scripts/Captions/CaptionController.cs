using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CaptionLibrary))]
public class CaptionController : MonoBehaviour
{
    public GameObject CaptionPrefab;

    public GameObject DescriptionPanel;

    public bool captionActive = true;

    CaptionLibrary CaptionLib;
    List<GameObject> CaptionList;

    // Start is called before the first frame update
    void Start()
    {
        CaptionLib = GetComponent<CaptionLibrary>();
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

    public void HandleCaptions(bool active)
    {
        //CaptionControlButton = GetComponent<captionControlButton>();
        //CaptionControlButton.text = active ? "Show Captions" : "Hide Captions";

        foreach (var c in CaptionList)
        {
            c.SetActive(captionActive);
        }

        captionActive = !captionActive;
    }
    /// <summary>
    /// Creates a new caption object and places it in the world facing towards the player
    /// </summary>
    /// <param name="captionName">The text to be displayed on the caption</param>
    /// <param name="captionLocation">The Vector location of where the caption should be placed</param>
    public void CreateCaption(string captionName, Vector3 captionLocation)
    {
        GameObject captionGO = Instantiate(CaptionPrefab, captionLocation,
            Quaternion.LookRotation(captionLocation - CameraCache.Main.transform.position));
        Caption cap = captionGO.GetComponent<Caption>();
        // Setting primary title will initialize the caption object.
        // It will autonomously attempt to fill in its description and translation.
        cap.InitializeCaption(captionName.Split(":")[0], CaptionLib, this);

        CaptionList.Add(cap.gameObject);
    }
}
