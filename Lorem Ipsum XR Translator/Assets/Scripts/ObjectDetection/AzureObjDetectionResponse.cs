using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObjectDetection
{
    public class Rectangle
    {
        public int x;
        public int y;
        public int w;
        public int h;

        public Rectangle(JObject rectJson)
        {
            x = (int)rectJson["x"];
            y = (int)rectJson["y"];
            w = (int)rectJson["w"];
            h = (int)rectJson["h"];
        }
    }

    public class DetectedObject
    {
        public Rectangle rectangle;
        public string objectName;
        public float confidence;

        public DetectedObject(JObject objJson)
        {
            rectangle = new Rectangle((JObject)objJson["rectangle"]);
            objectName = (string)objJson["object"];
            confidence = (float)objJson["confidence"];
        }
    }

    public class Metadata
    {
        public int height;
        public int width;
        public string format;

        public Metadata(JObject metaJson)
        {
            height = (int)metaJson["height"];
            width = (int)metaJson["width"];
            format = (string)metaJson["format"];
        }
    }

    public class AzureObjDetectionResponse
    {
        public List<DetectedObject> objects;
        public string requestId;
        public Metadata metadata;
        public string modelVersion;

        public AzureObjDetectionResponse(string jsonResponse)
        {
            JObject jsonMainResponse = JObject.Parse(jsonResponse);
            requestId = (string)jsonMainResponse["requestId"];
            modelVersion = (string)jsonMainResponse["modelVersion"];
            metadata = new Metadata((JObject)jsonMainResponse["metadata"]);
            objects = new List<DetectedObject>();
            JArray objectsJson = (JArray)jsonMainResponse["objects"];
            foreach (JObject obj in objectsJson)
            {
                objects.Add(new DetectedObject(obj));
            }
        }
    }
}
