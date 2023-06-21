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

    private IObjectDetectorClient _objectDetectorClient;

    private void Start()
    {
        objCategory.text = "loading...";
        this._objectDetectorClient = new AzureObjectDetector("c2242d7717124c79baa26bd78f027d8b");
        StartCoroutine(this._objectDetectorClient.DetectObjects("https://obj-holo.cognitiveservices.azure.com/vision/v3.2/detect?model-version=latest", "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fstatic.businessinsider.com%2Fimage%2F5484d9d1eab8ea3017b17e29%2Fimage.jpg&f=1&nofb=1&ipt=d01225cfc3990e677d40b1793b24123fe936cee5f157c1fd49544618fc2806b9&ipo=images", this.CallSetSceneObjects));
    }

    private void CallSetSceneObjects(string responseText)
    {
        StartCoroutine(SetSceneObjects(responseText));
        //UpdatePosition();
    }

    private IEnumerator SetSceneObjects(string responseText)
    {
        string imgUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fstatic.businessinsider.com%2Fimage%2F5484d9d1eab8ea3017b17e29%2Fimage.jpg&f=1&nofb=1&ipt=d01225cfc3990e677d40b1793b24123fe936cee5f157c1fd49544618fc2806b9&ipo=images";
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            }
            else
            {
                Debug.LogError("Failed to load image from URL: " + imgUrl + "\nError: " + www.error);
            }
        }

        objCategory.text = responseText;
        //UpdatePositon();
    }

    private void UpdatePosition()
    {
        RectTransform imageRectTransform = image.GetComponent<RectTransform>();
        RectTransform tmpRectTransform = objCategory.GetComponent<RectTransform>();
        tmpRectTransform.position = tmpRectTransform.position + new Vector3(imageRectTransform.rect.width / 2, imageRectTransform.rect.height + 4, 0f);
    }
}
