using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Config;
using ObjectDetection;
using Translator;
using Feedback;


public class APICalls : MonoBehaviour
{

    private System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        rand = new System.Random();

        IObjectDetectorClient detector1 = new CustomObjectDetection("http://192.168.0.172:8080");
        IObjectDetectorClient detector2 = new AzureObjectDetector(Secrets.GetAzureImageRecognitionKey());

        ITranslatorClient translator1 = new CustomTranslator("http://192.168.0.172:8080");
        ITranslatorClient translator2 = new AzureTranslator(Secrets.GetAzureTranslatorKey(), "northeurope");


        if (rand.NextDouble() > 0.5)
        {
            Debug.Log("Use Custom ML Server");
            StartCoroutine(translator1.Translate("Hello!", "en", new string[] { "fr" }, this.ProcessTranslation));
            StartCoroutine(detector1.DetectObjects("resnet50", new Texture2D(500, 500), this.ProcessDetection));
        }
        else
        {
            Debug.Log("Use Azure ML Services");
            StartCoroutine(translator2.Translate("Hello!", "en", new string[] { "fr" }, this.ProcessTranslation));
            StartCoroutine(detector2.DetectObjects("https://obj-holo.cognitiveservices.azure.com/vision/v3.2/detect?model-version=latest", new Texture2D(500, 500), this.ProcessDetection));
        }


        //FeedbackClient client = new FeedbackClient(Secrets.GetFeedbackServerEndpoint());
        //StartCoroutine(client.PutFeedback("translation", "s2s", true));
    }

    private void ProcessTranslation(string[] vals)
    {
        Debug.Log("Original: " + vals[0]);
        Debug.Log("Translated: " + vals[1]);
    }

    private void ProcessDetection(List<DetectedObject> objects)
    {
        Debug.Log("Detected: " + objects.ToString());
    }

}
