using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using Translator;

namespace SST
{
    public class SpeechToTextDisplay : MonoBehaviour
    {
        [SerializeField]
        private AzureSpeechToText azureSpeechToText; // Reference to the AzureSpeechToText script

        [SerializeField]
        private ToolTip originalToolTip; // Reference to the ToolTip UI element for original text

        [SerializeField]
        private ToolTip translatedToolTip; // Reference to the ToolTip UI element for translated text

        // Azure Translator setup
        private AzureTranslator azureTranslator;
        private const string apiKey = "5878a06b7c2c4a66beed0915fe52a400";
        private const string location = "northeurope";
        private const string targetLanguage = "es"; // Example: Spanish. Change to your desired language.

        private void Awake()
        {
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
            // Call the SSTRequest method and get the recognized text or error message
            string recognizedText = await azureSpeechToText.SSTRequest();

            // Display the recognized text in the original ToolTip
            DisplayInToolTip(originalToolTip, recognizedText);

            // Translate and display in the translated ToolTip
            StartCoroutine(TranslateAndDisplay(recognizedText));
        }

        private IEnumerator TranslateAndDisplay(string text)
        {
            yield return azureTranslator.Translate(text, "en", new string[] { targetLanguage }, response =>
            {
                // Deserialize the root array
                TranslationResponse[] translationResponses = JsonUtility.FromJson<Wrapper>("{\"array\":" + response + "}").array;

                // Check if we got any responses
                if (translationResponses != null && translationResponses.Length > 0)
                {
                    TranslationResponse translationResponse = translationResponses[0];

                    // Check if the translationResponse contains translations
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
            });
        }

        private void DisplayInToolTip(ToolTip toolTip, string text)
        {
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


        [System.Serializable]
        public class TranslationResponse
        {
            public TranslationItem[] translations;
        }

        [System.Serializable]
        public class TranslationItem
        {
            public string text;
            public string to;
        }

        [System.Serializable]
        public class Wrapper
        {
            public TranslationResponse[] array;
        }
    }
}
