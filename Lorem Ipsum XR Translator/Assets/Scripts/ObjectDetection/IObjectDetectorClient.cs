using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ObjectDetection
{
    public interface IObjectDetectorClient
    {
        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<string> callback);
    }
}
