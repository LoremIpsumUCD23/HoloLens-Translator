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

        public Rectangle(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public Rectangle(JObject rectJson)
        {
            x = (int)rectJson["x"];
            y = (int)rectJson["y"];
            w = (int)rectJson["w"];
            h = (int)rectJson["h"];
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

        public DetectedObject(Rectangle rectangle, string objectName, float confidence) 
        {
            this.rectangle = rectangle;
            this.objectName = objectName;
            this.confidence = confidence;   
        }

        public DetectedObject(JObject objJson)
        {
            rectangle = new Rectangle((JObject)objJson["rectangle"]);
            objectName = (string)objJson["object"];
            confidence = (float)objJson["confidence"];
        }
    }
}

