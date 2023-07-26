using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace SST
{
    public class SpeechToTextDisplay : MonoBehaviour
    {
        [SerializeField]
        private AzureSpeechToText azureSpeechToText; // Reference to the AzureSpeechToText script

        [SerializeField]
        private ToolTip toolTip; // Reference to the ToolTip UI element

        public async void OnRequestSpeechToText()
        {
            // Call the SSTRequest method and get the recognized text or error message
            string recognizedText = await azureSpeechToText.SSTRequest();

            // Display the recognized text in the ToolTip
            DisplayInToolTip(recognizedText);
        }

        private void DisplayInToolTip(string text)
        {
            toolTip.ToolTipText = text;
            toolTip.gameObject.SetActive(true);
        }
    }
}