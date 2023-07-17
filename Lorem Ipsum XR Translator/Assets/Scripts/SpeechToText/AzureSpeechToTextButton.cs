using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;

public class AzureSpeechToTextButton : MonoBehaviour
{
    private string speechServiceAPIKey = "1272512de9d14e4ebcab8f56b9bfaeb1";
    private string serviceRegion = "northeurope";

    public async void OnButtonPressed()
    {
        Debug.Log("Button was pressed.");
        SpeechConfig config = SpeechConfig.FromSubscription(speechServiceAPIKey, serviceRegion);
        using (var recognizer = new SpeechRecognizer(config))
        {
            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                Debug.Log($"We recognized: {result.Text}");
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                Debug.Log($"NOMATCH: Speech could not be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Debug.Log($"CANCELED: Did you update the subscription info?");
                }
            }
        }
    }
}
