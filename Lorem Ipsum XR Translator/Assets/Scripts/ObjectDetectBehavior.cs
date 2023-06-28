using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using ObjectDetection;
using Config;

public class ObjectDetectBehavior : MonoBehaviour
{
    public TextMeshPro objCategory;
    public GameObject imageQuad;

    private IObjectDetectorClient _objectDetectorClient;

    private void Start()
    {
        objCategory.text = "loading...";
        this._objectDetectorClient = new AzureObjectDetector(Secrets.GetAzureImageRecognitionKey());
        StartCoroutine(this._objectDetectorClient.DetectObjects("https://obj-holo.cognitiveservices.azure.com/vision/v3.2/detect?model-version=latest", "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi5.walmartimages.com%2Fasr%2Fa2e1cf08-bd39-43aa-9a9a-2edac8d9623d.e027078e8cd1d2f0b75ee1c6c9344fbb.jpeg&f=1&nofb=1&ipt=12b04e7da56462e66cbd9871b14ac78607713aa2db1ba7e1278da15171f364ab&ipo=images", this.CallSetSceneObjects));
    }

    private void CallSetSceneObjects(string responseText)
    {
        StartCoroutine(SetSceneObjects(responseText));
    }

    private IEnumerator SetSceneObjects(string responseText)
    {
        string imgUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi5.walmartimages.com%2Fasr%2Fa2e1cf08-bd39-43aa-9a9a-2edac8d9623d.e027078e8cd1d2f0b75ee1c6c9344fbb.jpeg&f=1&nofb=1&ipt=12b04e7da56462e66cbd9871b14ac78607713aa2db1ba7e1278da15171f364ab&ipo=images";
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                imageQuad.GetComponent<Renderer>().material.mainTexture = texture;
                //image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            }
            else
            {
                Debug.LogError("Failed to load image from URL: " + imgUrl + "\nError: " + www.error);
            }
        }

        objCategory.text = responseText;
    }
}
