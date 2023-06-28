using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ObjectDetection
{
    public interface IObjectDetectorClient
    {
        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<List<DetectedObject>> callback);
        public IEnumerator DetectObjects(string modelPath, Texture2D image, Action<List<DetectedObject>> callback);
    }
}
