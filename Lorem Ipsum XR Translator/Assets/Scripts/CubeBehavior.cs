using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

// our own namespace
using Description;
using Translator;

public class CubeBehavior : MonoBehaviour, IMixedRealityGestureHandler
{
    public TextMeshPro textObject;
    public TextMeshPro textobj;

    private IDescriptionClient _descriptionClient;
    private ITranslatorClient _translatorClient;


    void Start()
    {
        Debug.Log("Started");
        textObject.text = "Sugai";
        textobj.text = "tintin";

        //// Initialise decriptor client
        //// TODO: Check your API KEY here: https://platform.openai.com/account/api-keys
        //this._descriptionClient = new ChatGPTClient("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", "text-davinci-003");
        //// Get a description.
        //StartCoroutine(this._descriptionClient.SendRequest("Good morning", this.getDescription));

        // Elementary dictionary API key    : 50975cc1-b4e7-4666-af66-03067dc6060f  || ("elementary", "50975cc1-b4e7-4666-af66-03067dc6060f")
        // Intermediate dictionary API key  : 6009aa88-c0ad-49ef-97fe-c2e785c7d0a8  || ("intermediate", "6009aa88-c0ad-49ef-97fe-c2e785c7d0a8")
        this._descriptionClient = new DictionaryAPIClient("intermediate", "6009aa88-c0ad-49ef-97fe-c2e785c7d0a8");
        // Get a description.
        StartCoroutine(this._descriptionClient.SendRequest("morning", this.getDescription));

        // Initialise translator client
        this._translatorClient = new AzureTranslator("5878a06b7c2c4a66beed0915fe52a400","northeurope");
        // Get a translation in Japanese and Hindi.
        StartCoroutine(this._translatorClient.Translate("Good morning!", "en", new string[] { "ja", "hi" }, this.getTranslation));
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");
        StartCoroutine(this._descriptionClient.SendRequest("Good afternoon!", this.getDescription));
    }

    public void OnGestureUpdated(InputEventData eventData)
    {            
        Debug.Log("Gesture Updated");
    }
 
    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log("Gesture Completed");
        StartCoroutine(this._descriptionClient.SendRequest("Good evening!", this.getDescription));
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log("Gesture Canceled");
    }

    private void getDescription(string responseText)
    {
        if (responseText != null)
        {
            textObject.text = responseText;
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Description API client's implementation");
        }
    }

    private void getTranslation(string responseText)
    {
        if (responseText != null)
        {
            textobj.text = responseText;
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Translation API client's implementation");
        }
    }
}
