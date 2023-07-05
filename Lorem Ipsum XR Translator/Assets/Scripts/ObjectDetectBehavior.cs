using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using ObjectDetection;

public class ObjectDetectBehavior : MonoBehaviour
{
    public TextMeshPro objCategory;
    public Image image;
    public GameObject borderPrefab;
    public float borderThickness = 2;

    private IObjectDetectorClient _objectDetectorClient;

    private void Start()
    {        
        objCategory.text = "loading...";
        //this._objectDetectorClient = new AzureObjectDetector("c2242d7717124c79baa26bd78f027d8b");
        this._objectDetectorClient = new CustomObjectDetector();
        string baseModelPath = Path.Combine(Application.dataPath, "Scripts/ObjectDetection/PythonCustomModel").Replace("\\", "/");
        //StartCoroutine(this._objectDetectorClient.DetectObjects("https://obj-holo.cognitiveservices.azure.com/vision/v3.2/detect?model-version=latest", "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi5.walmartimages.com%2Fasr%2Fa2e1cf08-bd39-43aa-9a9a-2edac8d9623d.e027078e8cd1d2f0b75ee1c6c9344fbb.jpeg&f=1&nofb=1&ipt=12b04e7da56462e66cbd9871b14ac78607713aa2db1ba7e1278da15171f364ab&ipo=images", this.CallSetSceneObjects));
        StartCoroutine(this._objectDetectorClient.DetectObjects(baseModelPath, "examples/desk.jpeg", this.CallSetSceneObjects));
    }

    private void CallSetSceneObjects(string responseText)
    {
        StartCoroutine(SetSceneObjects(responseText));
    }

    private IEnumerator SetSceneObjects(string responseText)
    {
        //string imgUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi5.walmartimages.com%2Fasr%2Fa2e1cf08-bd39-43aa-9a9a-2edac8d9623d.e027078e8cd1d2f0b75ee1c6c9344fbb.jpeg&f=1&nofb=1&ipt=12b04e7da56462e66cbd9871b14ac78607713aa2db1ba7e1278da15171f364ab&ipo=images";
        //using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgUrl))
        //{
        //    yield return www.SendWebRequest();

        //    GC.Collect();
        //    if (www.result == UnityWebRequest.Result.Success)
        //    {
        //        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        //        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        //    }
        //    else
        //    {
        //        Debug.LogError("Failed to load image from URL: " + imgUrl + "\nError: " + www.error);
        //    }
        //}
        
        objCategory.text = "";
        if (!responseText.Equals("object"))
        {
            AzureObjDetectionResponse responseData = new AzureObjDetectionResponse(responseText);
            List<DetectedObject> detectedObjects = responseData.objects;

            float xScale = image.rectTransform.rect.width / responseData.metadata.width;
            float yScale = image.rectTransform.rect.height / responseData.metadata.height;
            float offsetX = image.rectTransform.rect.width / 2f;
            float offsetY = image.rectTransform.rect.height / 2f;
            
            foreach (DetectedObject detectedObject in detectedObjects)
            {
                float borderWidth = detectedObject.rectangle.w * xScale;
                float borderHeight = detectedObject.rectangle.h * yScale;
                float borderX = (detectedObject.rectangle.x * xScale) - offsetX + (borderWidth/2f);
                float borderY = offsetY - (borderHeight / 2f) - (detectedObject.rectangle.y * yScale);
                
                GameObject border = GameObject.Instantiate(borderPrefab);

                border.transform.SetParent(image.transform);
                border.transform.localPosition = new Vector3(borderX, borderY, 2);

                border.transform.localScale = new Vector3(borderWidth, borderHeight, 1f);

                GameObject textObject = new GameObject("ObjectName", typeof(TextMeshPro));
                TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
                textMesh.text = detectedObject.objectName + ": " + detectedObject.confidence;
                textMesh.fontSize = 15;
                textMesh.alignment = TextAlignmentOptions.Center;
                textMesh.color = Color.red;
                textObject.transform.SetParent(border.transform);

                textObject.transform.localPosition = new Vector3(0f, 0.6f, 0f);
                Debug.Log(textMesh.text);
            }
        }
        yield return null;
    }
}
