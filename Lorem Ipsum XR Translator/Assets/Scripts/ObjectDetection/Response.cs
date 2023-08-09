using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObjectDetection
{
    /// <summary>
    /// Rectangle helper class using x,y coordinates with w,h dimensions
    /// </summary>
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

        public Rectangle(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }

    /// <summary>
    /// Detected Object contains a rectangle location, object name, and confidence level
    /// </summary>
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

        public DetectedObject(Rectangle rec, string name, float confidence)
        {
            this.rectangle = rec;
            this.objectName = name;
            this.confidence = confidence;
        }
    }

    /// <summary>
    /// Metadata associated with image detection
    /// </summary>
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

    /// <summary>
    /// Response parser for Azure Object Detection
    /// </summary>
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

    /// <summary>
    /// Response parser for Custom Object Detection
    /// </summary>
    public class CustomObjDetectionResponse
    {
        public List<DetectedObject> objects;

        public CustomObjDetectionResponse(string jsonResponse)
        {
            JObject jsonMainResponse = JObject.Parse(jsonResponse);

            objects = new List<DetectedObject>();
            JArray objectsJson = (JArray)jsonMainResponse["predictions"];
            foreach (JObject obj in objectsJson)
            {
                objects.Add(new DetectedObject(
                  new Rectangle((int)obj["x"], (int)obj["y"], (int)obj["w"], (int)obj["h"]),
                  (string)obj["label"],
                  (float)obj["confidence"])
                );
            }
        }
    }

    public class GCPObjDetectionResponse
    {
        public List<DetectedObject> objects = new List<DetectedObject>();

        public GCPObjDetectionResponse(string res, int origWidth, int origHeight)
        {
            JObject resObj = JObject.Parse(res);
            JArray responses = (JArray)resObj["responses"];
            if (responses == null || responses.Count == 0)
            {
                return;
            }

            JObject firstObj = (JObject)responses[0];
            JArray annotations = (JArray)firstObj["localizedObjectAnnotations"];
            if (annotations == null || annotations.Count == 0)
            {
                return;
            }

            foreach (JObject obj in annotations)
            {
                JObject boundingPoly =  (JObject)obj["boundingPoly"];
                JArray normalizedVertices = (JArray)boundingPoly["normalizedVertices"];
                // TODO: Calculate rectangles using origWidth and origHeight
                objects.Add(new DetectedObject(
                  new Rectangle(
                      (int)(origWidth  *  (float)normalizedVertices[0]["x"]),
                      (int)(origHeight *  (float)normalizedVertices[0]["y"]),
                      (int)(origWidth  * ((float)normalizedVertices[1]["x"] - (float)normalizedVertices[0]["x"])),
                      (int)(origHeight * ((float)normalizedVertices[2]["y"] - (float)normalizedVertices[0]["y"]))),
                  (string)obj["name"],
                  (float)obj["score"])
                );
            }
        }
    }
}
