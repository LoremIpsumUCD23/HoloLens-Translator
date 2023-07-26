using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Config;
using System.Threading.Tasks;

namespace STT
{
    public class AzureSpeechToText : MonoBehaviour
    {
        private string serviceRegion = "northeurope";

        public async Task<string> SSTRequest()
        {
            Debug.Log("Button was pressed.");

            SpeechConfig config = SpeechConfig.FromSubscription(Secrets.GetSTTSpeechSDKKey(), serviceRegion);
            using (SpeechRecognizer recognizer = new SpeechRecognizer(config))
            {
                SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Debug.Log($"We recognized: {result.Text}");
                    return result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Debug.Log($"NOMATCH: Speech could not be recognized.");
                    return "NOMATCH: Speech could not be recognized.";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    CancellationDetails cancellation = CancellationDetails.FromResult(result);
                    Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Debug.Log($"CANCELED: Did you update the subscription info?");
                        return $"CANCELED: ErrorDetails={cancellation.ErrorDetails}";
                    }
                    return $"CANCELED: Reason={cancellation.Reason}";
                }
            }
            return "Unknown error occurred.";
        }
    }
}
