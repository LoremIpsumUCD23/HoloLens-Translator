using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(CaptionLibrary))]
public class CaptionController : MonoBehaviour
{
    public GameObject CaptionPrefab;

    public GameObject DescriptionPanel;

    CaptionLibrary CaptionLib;
    
    List<GameObject> ActiveCaptions;
    ObjectPool<GameObject> CaptionPool;

    // Start is called before the first frame update
    void Start()
    {
        CaptionLib = GetComponent<CaptionLibrary>();
        ActiveCaptions = new List<GameObject>();

        CaptionPool = new ObjectPool<GameObject>(OnCaptionCreate, OnCaptionGet, OnCaptionRelease, OnCaptionDestroy);
    }

    /// <summary>
    /// Deletes all current captions in world and clears the list
    /// </summary>
    public void ClearCaptions()
    {
        foreach (GameObject go in ActiveCaptions)
        {
            //AvailableCaptionList.Enqueue(go);
            CaptionPool.Release(go);
            //go.SetActive(false);
        }
        ActiveCaptions.Clear();
    }

    /// <summary>
    /// Removes a specified caption that the user has requested to close.
    /// </summary>
    /// <param name="caption">Caption reference to be removed</param>
    public void RemoveCaption(Caption caption)
    {
        CaptionPool.Release(caption.gameObject);

        // Need to find caption in current caption list and remove it. Unfortunately O(n) operation
        // This should be fine since caption list should never be particularly large.
        ActiveCaptions.Remove(caption.gameObject);
    }

    /// <summary>
    /// Creates a new caption object and places it in the world facing towards the player
    /// </summary>
    /// <param name="captionName">The text to be displayed on the caption</param>
    /// <param name="captionLocation">The Vector location of where the caption should be placed</param>
    public Caption CreateCaption(string captionName, Vector3 captionLocation)
    {
        // Check the pool for available captions first. If none, make a new one.
        GameObject captionGO = CaptionPool.Get();

        // Configure our newly created or collected caption
        captionGO.transform.position = captionLocation;
        captionGO.transform.rotation = Quaternion.LookRotation(captionLocation - CameraCache.Main.transform.position);
        Caption cap = captionGO.GetComponent<Caption>();

        // Setting primary title will initialize the caption object.
        // It will autonomously attempt to fill in its description and translation.
        cap.InitializeCaption(captionName.Split(":")[0], CaptionLib, this);

        ActiveCaptions.Add(cap.gameObject);

        // Return the newly created caption object if desired (primarily for testing).
        return cap;
    }

    #region Pooling Callbacks

    /// <summary>
    /// Called when a new caption object needs to be created by the caption pool.
    /// </summary>
    /// <returns>GameObject of newly instantiated CaptionPrefab object</returns>
    GameObject OnCaptionCreate()
    {
        return Instantiate(CaptionPrefab);
    }

    /// <summary>
    /// Called when a caption is requested. Sets the gameobject to be active.
    /// </summary>
    /// <param name="cap">Caption game object being requested</param>
    void OnCaptionGet(GameObject cap)
    {
        cap.SetActive(true);
    }

    /// <summary>
    /// Called when caption is released. Sets gameobject to be inactive.
    /// </summary>
    /// <param name="cap">Caption game object being released</param>
    void OnCaptionRelease(GameObject cap)
    {
        cap.SetActive(false);
    }

    /// <summary>
    /// Destroys the caption game object.
    /// </summary>
    /// <param name="cap">Caption game object to be destroyed</param>
    void OnCaptionDestroy(GameObject cap)
    {
        Destroy(cap);
    }

    #endregion

    /// <summary>
    /// Clean out our pool on application quit.
    /// </summary>
    private void OnApplicationQuit()
    {
        foreach (GameObject go in ActiveCaptions)
        {
            CaptionPool.Release(go);
        }
        ActiveCaptions.Clear();
        CaptionPool.Clear();
    }
}
