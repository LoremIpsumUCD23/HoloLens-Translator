using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

// our own namespace
using Description;
using Translator;
using Config;
using Util.Cache;


public class CubeBehavior : MonoBehaviour, IMixedRealityGestureHandler
{
    public TextMeshPro TranslationText;
    public TextMeshPro DictionaryText;
    public TextMeshPro ChatGPTText;

    private ITranslatorClient _translatorClient;
    private IDescriptionClient _chatGPTClient;
    private IDescriptionClient _dictionaryClient;


    private ICache<string, string>_cache;


    // for testing.
    private string[] objects = { "cup", "dog", "human", "rocket", "tree" };


    void Start()
    {
        Debug.Log("Started");

        // Cache usage. It returns a falsy value if the key doesn't exist.
        this._cache = new LRUCache<string, string>(10);
        this._cache.Put("Key", "Value");
        this._cache.Put("Key", "Updated Value");
        this._cache.Put("", "How about using an empty string as a key?");
        Debug.Log("Fake Key: " + this._cache.Get("Fake Key"));
        Debug.Log("Key: " + this._cache.Get("Key"));

        var rnd = new System.Random();
        int index = rnd.Next(objects.Length);
        string target = this.objects[index];
        string originalLanguage = "en";
        string[] targetLanguages = new string[]{ "bn" , "zh-Hant" };


        // Initialise translator client
        this._translatorClient = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");
        // Get a translation in French.
        StartCoroutine(this._translatorClient.Translate(target, originalLanguage, targetLanguages, this.GetTranslation));

        // Initialise decription client
        this._dictionaryClient = new DictionaryAPIClient("elementary", Secrets.GetDictApiKeyFor("elementary"));
        // Get a description.
        StartCoroutine(this._dictionaryClient.Explain(target, this.GetDescriptionFromDict));

        // Initialise decription client
        this._chatGPTClient = new ChatGPTClient(Secrets.GetChatGPTApiKey(), "text-davinci-003");
        // Get a description.
        string prompt = "Definition of " + target;
        StartCoroutine(this._chatGPTClient.Explain(prompt, this.GetDescriptionFromGPT));
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");

        var rnd = new System.Random();
        int index = rnd.Next(objects.Length);
        string target = this.objects[index];
        string originalLanguage = "en";
        string[] targetLanguages = new string[]{ "bn" , "ja" };


        // Initialise translator client
        this._translatorClient = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");
        // Get a translation in French.
        StartCoroutine(this._translatorClient.Translate(target, originalLanguage, targetLanguages, this.GetTranslation));

        // Initialise decription client
        this._dictionaryClient = new DictionaryAPIClient("elementary", Secrets.GetDictApiKeyFor("elementary"));
        // Get a description.
        StartCoroutine(this._dictionaryClient.Explain(target, this.GetDescriptionFromDict));

        // Initialise decription client
        this._chatGPTClient = new ChatGPTClient(Secrets.GetChatGPTApiKey(), "text-davinci-003");
        // Get a description.
        string prompt = "Definition of " + target;
        StartCoroutine(this._chatGPTClient.Explain(prompt, this.GetDescriptionFromGPT));

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
