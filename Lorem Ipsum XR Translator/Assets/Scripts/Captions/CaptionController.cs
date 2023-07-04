using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using Description;
using Translator;
using Config;

public class CaptionController : MonoBehaviour
{
    public GameObject CaptionPrefab;

    List<GameObject> CaptionList;

    private ITranslatorClient _translatorClient;
    private IDescriptionClient _chatGPTClient;
    private IDescriptionClient _dictionaryClient;

    // This should be moved to some other translation settings class later.
    string originalLanguage = "en";
    string[] targetLanguages = new string[] { "ja" };

    // Start is called before the first frame update
    void Start()
    {
        CaptionList = new List<GameObject>();

        // Initialise translator client
        this._translatorClient = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");
        // Get a translation in French.
        //StartCoroutine(this._translatorClient.Translate(target, originalLanguage, targetLanguages, this.GetTranslation));

        // Initialise description client
        this._dictionaryClient = new DictionaryAPIClient("elementary", Secrets.GetDictApiKeyFor("elementary"));
        // Get a description.

        // Initialise description client
        this._chatGPTClient = new ChatGPTClient(Secrets.GetChatGPTApiKey(), "text-davinci-003");
        // Get a description.
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
        GameObject captionGO = Instantiate(CaptionPrefab, captionLocation, 
            Quaternion.LookRotation(captionLocation - CameraCache.Main.transform.position));
        Caption cap = captionGO.GetComponent<Caption>();
        cap.SetPrimaryTitle(captionName.Split(":")[0]);

        // Get Description
        StartCoroutine(this._dictionaryClient.Explain(cap, this.GetDescriptionFromDict));
    }

    private void GetDescriptionFromDict(Caption caption)
    {
        if (caption.GetPrimaryDescription() != null)
        {
            StartCoroutine(this._translatorClient.Translate(caption, originalLanguage, targetLanguages, this.GetTranslation));
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Description API client's implementation. Try ChatGPT instead");

            //string prompt = "Definition of " + target;
            StartCoroutine(this._chatGPTClient.Explain(caption, this.GetDescriptionFromGPT));
        }
    }

    private void GetDescriptionFromGPT(Caption caption)
    {
        if (caption.GetPrimaryDescription() != null)
        {
            StartCoroutine(this._translatorClient.Translate(caption, originalLanguage, targetLanguages, this.GetTranslation));
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Description API client's implementation");
        }
    }
    private void GetTranslation(Caption caption)
    {
        if (caption.GetTranslatedDescription() != null)
        {
            FinalizeCaption(caption);
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Translation API client's implementation");
        }
    }

    private void FinalizeCaption(Caption cap)
    {
        CaptionList.Add(cap.gameObject);
    }
}
