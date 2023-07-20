using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using Config;

public class AzureSpeechToTextButton : MonoBehaviour
{
    private string serviceRegion = "northeurope";

    public ToolTip toolTip;

    public async void OnButtonPressed()
    {
        Debug.Log("Button was pressed.");

        SpeechConfig config = SpeechConfig.FromSubscription(Secrets.GetSTTSpeechSDKKey(), serviceRegion);
        using (var recognizer = new SpeechRecognizer(config))
        {
            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                Debug.Log($"We recognized: {result.Text}");
                toolTip.ToolTipText = result.Text;
                toolTip.gameObject.SetActive(true);
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

