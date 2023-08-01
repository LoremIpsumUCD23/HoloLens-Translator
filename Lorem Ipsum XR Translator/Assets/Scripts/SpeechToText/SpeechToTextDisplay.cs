using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using Translator;
using Config;

namespace STT
{
    public class SpeechToTextDisplay : MonoBehaviour
    {
        [SerializeField]
        private AzureSpeechToText azureSpeechToText;

        [SerializeField]
        private ToolTip originalToolTip;

        [SerializeField]
        private ToolTip translatedToolTip;

        private AzureTranslator azureTranslator;
        private string apiKey;
        private const string location = "northeurope";
        private const string targetLanguage = "hi";

        private void Awake()
        {
            apiKey = Secrets.GetAzureTranslatorKey();
            azureTranslator = new AzureTranslator(apiKey, location);
        }

        private void Start()
        {
            if (originalToolTip != null)
            {
                originalToolTip.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Original ToolTip is not assigned.");
            }

            if (translatedToolTip != null)
            {
                translatedToolTip.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Translated ToolTip is not assigned.");
            }
        }

        public async void OnRequestSpeechToText()
        {
            string recognizedText = await azureSpeechToText.SSTRequest();
            DisplayInToolTip(originalToolTip, recognizedText);
            StartCoroutine(TranslateAndDisplay(recognizedText));
        }

        private IEnumerator TranslateAndDisplay(string text)
        {
            IEnumerator translationRoutine = PerformTranslation(text, "en", new string[] { targetLanguage });
            while (translationRoutine.MoveNext())
            {
                yield return translationRoutine.Current;
            }
        }

       private IEnumerator PerformTranslation(string text, string fromLanguage, string[] targetLanguages)
{
    yield return azureTranslator.Translate(text, fromLanguage, targetLanguages, response =>
    {
        if (response.Length == 0)
        {
            Debug.LogError("Empty response received.");
            return;
        }

        string jsonResponse;
        if (response.Length > 1) 
        {
            jsonResponse = response[1]; // Assuming the second item is the translated text
        } 
        else 
        {
            jsonResponse = response[0];
        }
        Debug.Log("Received JSON: " + jsonResponse);

        // Check if JSON is an array or not
        if (!jsonResponse.StartsWith("[") || !jsonResponse.EndsWith("]"))
        {
            // If the response is not an array, assume it's a direct translation
            DisplayInToolTip(translatedToolTip, jsonResponse);
            return;
        }

        // If the response is an array, deserialize as previously
        try
        {
            TranslationResponse[] translationResponses = JsonUtility.FromJson<Wrapper>("{\"array\":" + jsonResponse + "}").array;

            if (translationResponses != null && translationResponses.Length > 0)
            {
                TranslationResponse translationResponse = translationResponses[0];

                if (translationResponse.translations != null && translationResponse.translations.Length > 0)
                {
                    string translatedText = translationResponse.translations[0].text;
                    DisplayInToolTip(translatedToolTip, translatedText);
                }
                else
                {
                    Debug.LogError("Failed to extract translation from response.");
                }
            }
            else
            {
                Debug.LogError("No translation responses received.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to deserialize JSON: " + e.Message);
        }
    });
}




        private void DisplayInToolTip(ToolTip toolTip, string text)
        {
            Debug.Log("Displaying in Tooltip: " + text);
            if (toolTip != null)
            {
                toolTip.ToolTipText = text;
                toolTip.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("ToolTip is not assigned.");
            }
        }
    }

[System.Serializable]
public class TranslationItem
{
    public string text;
    public string to;
}

[System.Serializable]
public class TranslationResponse
{
    public TranslationItem[] translations;
}

[System.Serializable]
public class Wrapper
{
    public TranslationResponse[] array;
}
}
