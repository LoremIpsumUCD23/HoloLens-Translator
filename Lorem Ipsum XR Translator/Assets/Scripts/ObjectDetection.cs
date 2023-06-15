using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ObjectDetection : MonoBehaviour
{
    public TextMeshPro objCategory;
    private readonly string subscriptionKey = "c2242d7717124c79baa26bd78f027d8b";
    private readonly string azureObjApi = "https://obj-holo.cognitiveservices.azure.com/vision/v3.2/analyze?visualFeatures=Categories&language=en&model-version=latest";
    // Start is called before the first frame update
    private void Start()
    {
        objCategory.text = "object";
        StartCoroutine(DetectObject());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DetectObject()
    {
        // Request body
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes("{'url':'https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fstatic.businessinsider.com%2Fimage%2F5484d9d1eab8ea3017b17e29%2Fimage.jpg&f=1&nofb=1&ipt=d01225cfc3990e677d40b1793b24123fe936cee5f157c1fd49544618fc2806b9&ipo=images'}");

        UnityWebRequest client = new UnityWebRequest(azureObjApi, "POST");

        client.uploadHandler = new UploadHandlerRaw(byteData);
        client.downloadHandler = new DownloadHandlerBuffer();

        // Request headers
        client.SetRequestHeader("Content-Type", "application/json");
        client.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);

        yield return client.SendWebRequest();

        if (client.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("API request failed. Error: " + client.error);
        }
        else
        {
            ObjDetectionResponse responseData = JsonUtility.FromJson<ObjDetectionResponse>(client.downloadHandler.text);

            // Access the categories
            Category[] categories = responseData.categories;

            string categoryName = "object";
            float categoryScore = 0;

            foreach (Category category in categories) {
                if (category.score > categoryScore) {
                    categoryScore = category.score;
                    categoryName = category.name;
                }
            }
            objCategory.text = categoryName+": "+categoryScore;
            Debug.Log("API response: " + client.downloadHandler.text);
        }
    }
}
