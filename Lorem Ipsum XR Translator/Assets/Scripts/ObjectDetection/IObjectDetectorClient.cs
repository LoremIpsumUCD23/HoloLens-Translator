using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ObjectDetection
{
    /// <summary>
    /// Interface for detecting objects by URL or via a Texture2D image
    /// </summary>
    public interface IObjectDetectorClient 
    {
        IEnumerator DetectObjects(string imagePath, Action<List<DetectedObject>> callback);
        IEnumerator DetectObjects(Texture2D image, Action<List<DetectedObject>> callback);
    }
}
