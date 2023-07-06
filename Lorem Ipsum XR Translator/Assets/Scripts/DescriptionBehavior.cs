using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

// our own namespace
using Description;
using Translator;
using Config;

public class DescriptionBehavior : MonoBehaviour, IMixedRealityGestureHandler
{
    [SerializeField] private TextMeshPro TranslationText;
    [SerializeField] private TextMeshPro DictionaryText;
    [SerializeField] private TextMeshPro ChatGPTText;

    public GameObject DescriptionPanel;

    private ITranslatorClient _translatorClient;
    private IDescriptionClient _chatGPTClient;
    private IDescriptionClient _dictionaryClient;


    // for testing.
    // private string[] objects = { "cup", "dog", "human", "rocket", "tree" };
    [SerializeField] private TextMeshPro Title;

    [SerializeField] private string word; // word to be described
    [SerializeField] private string secondarylanguage; // chosen secondary language

    void Start()
    {
        Debug.Log("Initialising");

        word = "cube";
        secondarylanguage = "fr";

        // Initialise translator client
        this._translatorClient = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");

        // Initialise decription client
        this._dictionaryClient = new DictionaryAPIClient("elementary", Secrets.GetDictApiKeyFor("elementary"));

        // Initialise decription client
        this._chatGPTClient = new ChatGPTClient(Secrets.GetChatGPTApiKey(), "text-davinci-003");
    }

    public void OpenDescriptionPanel()
    {
        DescriptionPanel.SetActive(true);
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");

        Title.text = word;
        string target = word;
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

    public void RetrieveDescription()
    {
        Debug.Log("Retrieving Description");
        Title.text = word;
        string target = word;
        string originalLanguage = "en";
        string[] targetLanguages = new string[] { "fr" };

        // Get a translation
        StartCoroutine(this._translatorClient.Translate(target, originalLanguage, targetLanguages, this.GetTranslation));

        // Get a description from MerriamWebster.
        StartCoroutine(this._dictionaryClient.SendRequest(target, this.GetDescriptionFromDict));

        // Get a description from ChatGPT.
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
