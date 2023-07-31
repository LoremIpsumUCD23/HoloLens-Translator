using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Config;
using System.Threading.Tasks;

namespace STT
{
    /// <summary>
    /// Class AzureSpeechToText in STT namespace is used to convert Speech to text using Azure speech SDK
    /// </summary>
    public class AzureSpeechToText : MonoBehaviour
    {
        /// <summary>
        /// string serviceRegion represents the region selected in the azure services
        /// </summary>
        private string serviceRegion = "northeurope";

        /// <summary>
        ///SSTRequest() is a asynchronous method  and returns a Task<string>. This means when you call this method,
        ///it will run asynchronously, and once it's done, it will return a string.
        /// </summary>
        public async Task<string> SSTRequest()
        {
            

            /// <summary>
            /// Here a SpeechConfig object is created using a subscription key (retrieved from Secrets.GetSTTSpeechSDKKey()) and the service region.
            /// </summary>
            SpeechConfig config = SpeechConfig.FromSubscription(Secrets.GetSTTSpeechSDKKey(), serviceRegion);
            using (SpeechRecognizer recognizer = new SpeechRecognizer(config))
            {
                /// <summary>
                /// Here, the recognizer tries to recognize speech once (RecognizeOnceAsync()). This is an asynchronous operation, so it uses await to wait for the result.
                /// </summary>
                SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();

                
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    /// <summary>
                    /// Debug log gives message "We recognized:The message"
                    /// </summary>
                    Debug.Log($"We recognized: {result.Text}");
                    return result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    /// <summary>
                    /// Debug log gives message "NOMATCH: Speech could not be recognized"
                    /// </summary>
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
