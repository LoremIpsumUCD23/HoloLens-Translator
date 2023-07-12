using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObjectDetection
{
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
    }
}

