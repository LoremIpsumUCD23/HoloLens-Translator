using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using Translator;
using Config;  

namespace STT
{
    /// <summary>
    /// The class SpeechToTextDisplay is defined within the STT namespace is used to translate and display in the scene 
    /// </summary>
    public class SpeechToTextDisplay : MonoBehaviour
    {
        /// <summary>
        /// Calls the AzureSpeechToText script
        /// </summary>
        [SerializeField]
        private AzureSpeechToText azureSpeechToText; 

        
        /// <summary>
        /// originalToolTip is used to display the text converted from Speech to text
        /// </summary>
        [SerializeField]
        private ToolTip originalToolTip; 

        
        /// <summary>
        /// translatedToolTip is used to display the translated text
        /// </summary>
        [SerializeField]
        private ToolTip translatedToolTip; 

        /// <summary>
        /// Azure Translator setup
        /// </summary>
        private AzureTranslator azureTranslator;
        private string apiKey;  
        private const string location = "northeurope";
        private const string targetLanguage = "es"; 

        private void Awake()
        {
            apiKey = Secrets.GetAzureTranslatorKey();  
            azureTranslator = new AzureTranslator(apiKey, location);
        }

        
        /// <summary>
        /// This method is called before the first frame update in Unity. It checks if the tooltips are assigned and sets them to be inactive (hidden) initially.
        /// </summary>
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

         /// <summary>
        /// TranslateAndDisplay which is a asynchronous method is called when a user action (button press) occurs.
        /// It requests Azure's Speech to Text service and translation service to recognize speech and then displays the recognized and translated text in the tooltips.
        /// </summary>
        public async void OnRequestSpeechToText()
        {
            
            /// <summary>
            /// Call the SSTRequest method and get the recognized text or error message
            /// </summary>
            string recognizedText = await azureSpeechToText.SSTRequest();

            
            /// <summary>
            /// Display the recognized text in the original ToolTip
            /// </summary>
            DisplayInToolTip(originalToolTip, recognizedText);

            /// <summary>
            /// Translate and display in the translated ToolTip
            /// </summary>
            StartCoroutine(TranslateAndDisplay(recognizedText));
        }

        
        /// <summary>
        /// This method is a coroutine that takes the recognized text, sends it for translation, and then displays the translated text in the tooltip.
        /// </summary>
        private IEnumerator TranslateAndDisplay(string text)
        {
            yield return azureTranslator.Translate(text, "en", new string[] { targetLanguage }, response =>
            {
                // Deserialize the root array
                TranslationResponse[] translationResponses = JsonUtility.FromJson<Wrapper>("{\"array\":" + response + "}").array;

                
                /// <summary>
                /// Checks if we got any responses
                /// </summary>
                
                if (translationResponses != null && translationResponses.Length > 0)
                {
                    TranslationResponse translationResponse = translationResponses[0];

                    
                    /// <summary>
                    /// Checks if the translationResponse contains translations
                     /// </summary>
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

         /// <summary>
        /// DisplayInToolTip method sets the provided text to the given tooltip and makes the tooltip visible
        /// </summary>
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

         /// <summary>
        ///TranslationResponse,TranslationItem and Wrapper these are serializable helper classes used to deserialize the JSON response from the Azure Translator service.
        /// It requests Azure's Speech to Text service and translation service to recognize speech and then displays the recognized and translated text in the tooltips.
        /// </summary>
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
