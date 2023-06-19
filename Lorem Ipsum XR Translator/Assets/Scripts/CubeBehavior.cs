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
    public TextMeshPro TranslationText;
    public TextMeshPro DictionaryText;
    public TextMeshPro ChatGPTText;

    private ITranslatorClient _translatorClient;
    private IDescriptionClient _chatGPTClient;
    private IDescriptionClient _dictionaryClient;


    void Start()
    {
        Debug.Log("Started");

        string target = "cup";
        string originalLanguage = "en";
        string[] targetLanguages = new string[]{ "fr" };


        // Initialise translator client
        this._translatorClient = new AzureTranslator("5878a06b7c2c4a66beed0915fe52a400", "northeurope");
        // Get a translation in French.
        StartCoroutine(this._translatorClient.Translate(target, originalLanguage, targetLanguages, this.getTranslation));

        // Elementary dictionary API key    : 50975cc1-b4e7-4666-af66-03067dc6060f  || ("elementary", "50975cc1-b4e7-4666-af66-03067dc6060f")
        // Intermediate dictionary API key  : 6009aa88-c0ad-49ef-97fe-c2e785c7d0a8  || ("intermediate", "6009aa88-c0ad-49ef-97fe-c2e785c7d0a8")
        this._dictionaryClient = new DictionaryAPIClient("elementary", "50975cc1-b4e7-4666-af66-03067dc6060f");
        // Get a description.
        StartCoroutine(this._dictionaryClient.SendRequest(target, this.getDescriptionFromDict));

        // Initialise decription client
        // TODO: Check your API KEY here: https://platform.openai.com/account/api-keys
        this._chatGPTClient = new ChatGPTClient("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", "text-davinci-003");
        // Get a description.
        string prompt = "Definition of " + target;
        StartCoroutine(this._chatGPTClient.SendRequest(prompt, this.getDescriptionFromGPT));
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");
    }

    public void OnGestureUpdated(InputEventData eventData)
    {            
        Debug.Log("Gesture Updated");
    }
 
    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log("Gesture Completed");
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log("Gesture Canceled");
    }

    private void getTranslation(string responseText)
    {
        if (responseText != null)
        {
            TranslationText.text = "Translator: " + responseText;
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Translation API client's implementation");
        }
    }

    private void getDescriptionFromDict(string responseText)
    {
        if (responseText != null)
        {
            DictionaryText.text = "Dict: " + responseText;
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Description API client's implementation");
        }
    }

    private void getDescriptionFromGPT(string responseText)
    {
        if (responseText != null)
        {
            ChatGPTText.text = "GPT: " + responseText;
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Description API client's implementation");
        }
    }
}
