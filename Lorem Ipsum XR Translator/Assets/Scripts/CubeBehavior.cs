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

        // Initialise decriptor client
        // TODO: Check your API KEY here: https://platform.openai.com/account/api-keys
        this._descriptionClient = new ChatGPTClient("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", "text-davinci-003");
        // Get a description.
        StartCoroutine(this._descriptionClient.SendRequest("Good morning!", this.getDescription));


        // Initialise translator client
        this._translatorClient = new AzureTranslator(/*Maybe parameters here?*/);
        // Get a translation in Japanese.
        string language = "ja";
        StartCoroutine(this._translatorClient.Translate("Good morning!", language, this.getTranslation));

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

    private void getTranslation(string originalText)
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
