using Config;
using Description;
using System;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UnityEngine;
using Util;
using Util.Cache;

public class CaptionLibrary : MonoBehaviour 
{
    LRUCache<string, string> descriptions;
    LRUCache<string, string> titleTranslations;
    LRUCache<string, string> descriptionTranslations;

    private ITranslatorClient _translatorClient;
    private IDescriptionClient _chatGPTClient;
    private IDescriptionClient _dictionaryClient;

    int cacheCapacity = 100;


    string holdString = "Processing...";
    string separator = "<|>";

    // This should be moved to some other translation settings class later.
    string originalLanguage = "en";
    string[] targetLanguages = new string[] { "ja" };

    public string HoldString { get => holdString; set => holdString = value; }

    void Start()
    {
        // Initialize caches
        descriptions = new LRUCache<string, string>(cacheCapacity);
        titleTranslations = new LRUCache<string, string>(cacheCapacity);
        descriptionTranslations = new LRUCache<string, string>(cacheCapacity);

        // Initialise translator client
        this._translatorClient = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");

        // Initialise description client
        this._dictionaryClient = new DictionaryAPIClient("elementary", Secrets.GetDictApiKeyFor("elementary"));

        // Initialise description client
        this._chatGPTClient = new ChatGPTClient(Secrets.GetChatGPTApiKey(), "text-davinci-003");
    }

    public string TryGetDescription(string title)
    {
        string result = descriptions.Get(title);
        if (!string.IsNullOrEmpty(result))
        {
            return result;
        }
        else
        {
            // Request a new description
            descriptions.Put(title, holdString);
            StartCoroutine(_dictionaryClient.Explain(title, GetDescriptionFromDict));

            return null;
        }
    }

    public string TryGetTitleTranslation(string title)
    {
        return titleTranslations.Get(title);
    }

    public string TryGetDescriptionTranslation(string title)
    {
        return descriptionTranslations.Get(title);
    }

    private void GetDescriptionFromDict(string[] returned)
    {
        if (returned[1] != null)
        {
            descriptions.Put(returned[0], returned[1]);
            string toTranslate = returned[0] + separator + returned[1];
            StartCoroutine(_translatorClient.Translate(toTranslate, originalLanguage, targetLanguages, GetTranslation));
        }
        else
        {
            string result = "Got null. Must be something wrong with Description API client's implementation. Try ChatGPT instead";
            Debug.Log(result);

            string prompt = "Definition of " + returned[0];
            StartCoroutine(_chatGPTClient.Explain(prompt, GetDescriptionFromGPT));
        }
    }

    private void GetDescriptionFromGPT(string[] returned)
    {
        if (returned[1] != null)
        {
            descriptions.Put(returned[0], returned[1]);
            string toTranslate = returned[0] + separator + returned[1];
            StartCoroutine(_translatorClient.Translate(toTranslate, originalLanguage, targetLanguages, GetTranslation));
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Description API client's implementation");
        }
    }
    private void GetTranslation(string[] returned)
    {
        if (returned[1] != null)
        {
            string[] original = returned[0].Split(separator);
            string[] results = returned[1].Split(separator);
            titleTranslations.Put(original[0], results[0]);
            descriptionTranslations.Put(original[0], results[1]);
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Translation API client's implementation");
        }
    }
}
