using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

// our own namespace
using Description;
using Translator;
using Config;


public class CubeBehavior : MonoBehaviour, IMixedRealityGestureHandler
{
    public TextMeshPro TranslationText;
    public TextMeshPro DictionaryText;
    public TextMeshPro ChatGPTText;

    private ITranslatorClient _translatorClient;
    private IDescriptionClient _chatGPTClient;
    private IDescriptionClient _dictionaryClient;
    

    // for testing.
    private string[] objects = { "cup", "dog", "human", "rocket", "tree" };


    void Start()
    {
        Debug.Log("Started");

        var rnd = new System.Random();
        int index = rnd.Next(objects.Length);
        string target = this.objects[index];
        string originalLanguage = "en";
        string[] targetLanguages = new string[]{ "fr" };


        // Initialise translator client
        this._translatorClient = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");
        // Get a translation in French.
        StartCoroutine(this._translatorClient.Translate(target, originalLanguage, targetLanguages, this.GetTranslation));

        // Initialise decription client
        this._dictionaryClient = new DictionaryAPIClient("elementary", Secrets.GetDictApiKeyFor("elementary"));
        // Get a description.
        StartCoroutine(this._dictionaryClient.SendRequest(target, this.GetDescriptionFromDict));

        // Initialise decription client
        this._chatGPTClient = new ChatGPTClient(Secrets.GetChatGPTApiKey(), "text-davinci-003");
        // Get a description.
        string prompt = "Definition of " + target;
        StartCoroutine(this._chatGPTClient.SendRequest(prompt, this.GetDescriptionFromGPT));
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");

        var rnd = new System.Random();
        int index = rnd.Next(objects.Length);
        string target = this.objects[index];
        string originalLanguage = "en";
        string[] targetLanguages = new string[] { "fr" };


        // Initialise translator client
        this._translatorClient = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");
        // Get a translation in French.
        StartCoroutine(this._translatorClient.Translate(target, originalLanguage, targetLanguages, this.GetTranslation));

        // Initialise decription client
        this._dictionaryClient = new DictionaryAPIClient("elementary", Secrets.GetDictApiKeyFor("elementary"));
        // Get a description.
        StartCoroutine(this._dictionaryClient.SendRequest(target, this.GetDescriptionFromDict));

        // Initialise decription client
        this._chatGPTClient = new ChatGPTClient(Secrets.GetChatGPTApiKey(), "text-davinci-003");
        // Get a description.
        string prompt = "Definition of " + target;
        StartCoroutine(this._chatGPTClient.SendRequest(prompt, this.GetDescriptionFromGPT));

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

    private void GetTranslation(string responseText)
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

    private void GetDescriptionFromDict(string responseText)
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

    private void GetDescriptionFromGPT(string responseText)
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
