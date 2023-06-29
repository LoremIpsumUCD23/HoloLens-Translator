using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using ObjectDetection;
using Config;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ObjectDetectBehavior : MonoBehaviour
{
    public TextMeshPro objCategory;
    public GameObject imageQuad;
    public Texture2D image;

    public GameObject Caption;

    private IObjectDetectorClient _objectDetectorClient;

    private void Start()
    {
    }

    public void DetectImage(Texture2D image)
    {
        this.image = image;
        objCategory.text = "loading...";
        this._objectDetectorClient = new AzureObjectDetector(Secrets.GetAzureImageRecognitionKey());
        StartCoroutine(this._objectDetectorClient.DetectObjects("https://obj-holo.cognitiveservices.azure.com/vision/v3.2/detect?model-version=latest",
            image, this.CallSetSceneObjects));
    }

    private void CallSetSceneObjects(List<DetectedObject> detectedObjects)
    {
        string responseText = "";
        foreach (DetectedObject detectedObject in detectedObjects)
        {
            RaycastHit hit;
            Debug.Log(detectedObject.objectName + ": " + detectedObject.rectangle.x + "," + detectedObject.rectangle.y +
                " x " + detectedObject.rectangle.w + "," + detectedObject.rectangle.h +
                " Screen: " + Screen.width + "," + Screen.height);
            Vector2 averagePos = new Vector2(detectedObject.rectangle.x + detectedObject.rectangle.w / 2,
                Screen.height - detectedObject.rectangle.y - detectedObject.rectangle.h / 2);
            Debug.Log(detectedObject.objectName + " : " + averagePos.ToString());
            Ray ray = Camera.main.ScreenPointToRay(averagePos);
            Debug.DrawRay(ray.origin, ray.direction);
            Physics.Raycast(ray, out hit);
            GameObject caption = Instantiate(Caption);
            caption.transform.position = hit.point;
            caption.transform.Find("CaptionText").GetComponent<TextMeshPro>().text = detectedObject.objectName;
            caption.transform.rotation.SetLookRotation(Camera.main.transform.position, Vector3.up);
            responseText = responseText + detectedObject.objectName + ": " + detectedObject.confidence + "\n";
        }
        SetSceneObjects(responseText);
    }

    private void SetSceneObjects(string responseText)
    {
        imageQuad.GetComponent<Renderer>().material.mainTexture = image;

        objCategory.text = responseText;
    }
}
