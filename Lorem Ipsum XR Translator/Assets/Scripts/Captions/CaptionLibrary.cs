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
    // Three caches are set up for tracking descriptions and translated titles (primary titles are the key)
    ICache<string, string> descriptions;
    ICache<string, string> titleTranslations;
    ICache<string, string> descriptionTranslations;

    // Services for requesting descriptions/translations
    private ITranslatorClient _translatorClient;
    private IDescriptionClient _chatGPTClient;
    private IDescriptionClient _dictionaryClient;

    // Capacity for our caches (currently arbitrary)
    int cacheCapacity = 100;

    // Strings used for string processing logic (may be better ways to handle some of this)
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

    /// <summary>
    /// Attempts to get a description for a given title. If the caption is in the cache, it will be returned.
    /// If the cache returns a null, it will put in a request to the API to get a description, and set the value to the HoldString (to prevent multiple API calls)
    /// </summary>
    /// <param name="title">Caption title to look up</param>
    /// <returns>Hold String or a description</returns>
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

    /// <summary>
    /// Attempts to get a title translation
    /// </summary>
    /// <param name="title">Caption title in primary language</param>
    /// <returns>Hold String or translated title</returns>
    public string TryGetTitleTranslation(string title)
    {
        return titleTranslations.Get(title);
    }

    /// <summary>
    /// Attempts to get a translated description
    /// </summary>
    /// <param name="title">Caption title in primary language</param>
    /// <returns>Hold String or translated description</returns>
    public string TryGetDescriptionTranslation(string title)
    {
        return descriptionTranslations.Get(title);
    }

    /// <summary>
    /// Callback function for completed dictionary description API call. Assigns returned description to cache and requests translation.
    /// </summary>
    /// <param name="returned">String array containing primary title in index 0 and primary description in index 1</param>
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

    /// <summary>
    /// Callback function for completed GPT description API call. Assigns returned description to cache and requests translation.
    /// </summary>
    /// <param name="returned">String array containing primary title in index 0 and primary description in index 1</param>
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
    
    /// <summary>
    /// Callback function for completed translation API call. Parses returned results and assigns title and description translations to cache.
    /// </summary>
    /// <param name="returned">String array containing primary title in index 0 and translated title and description in index 1 separated by a separator string</param>
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