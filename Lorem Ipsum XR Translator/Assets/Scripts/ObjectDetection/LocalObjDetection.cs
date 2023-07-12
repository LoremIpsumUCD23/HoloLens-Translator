using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;


namespace ObjectDetection
{
    public class LocalObjDetection : IObjectDetectorClient
    {
        private IWorker _worker;

        private int _inputWidth;

        private int _inputHeight;


        public LocalObjDetection(NNModel modelAsset, int inputWidth, int inputHeight)
        {
            var runtimeModel = ModelLoader.Load(modelAsset);
            this._worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, runtimeModel);
            this._inputWidth = inputWidth;
            this._inputHeight = inputHeight;
        }


        public IEnumerator DetectObjects(string imagePath, Action<List<DetectedObject>> callback)
        {
            yield break;
        }


        public IEnumerator DetectObjects(Texture2D image, Action<List<DetectedObject>> callback)
        {
            Texture2D resized = LocalObjDetection.ResizeTexture(image, this._inputWidth, this._inputHeight);
            using (var tensor = new Tensor(resized, 3))
            {
                // Execute the model with the input tensor
                var output = this._worker.Execute(tensor).PeekOutput();
                yield return new WaitForCompletion(output);

                Debug.Log(output);

                // TODO: Extract data from the "output" variable. The "output" variable's type is Tensor. 
                // see: https://docs.unity3d.com/Packages/com.unity.barracuda@3.0/api/Unity.Barracuda.Tensor.html?q=Tensor
                // see: https://docs.unity3d.com/Packages/com.unity.barracuda@3.0/manual/TensorHandling.html
                List<DetectedObject> detectedObjects = new List<DetectedObject>();
                detectedObjects.Add(new DetectedObject(new Rectangle(0, 0, 0, 0), output.ToString(), 1.0f));

                callback(detectedObjects);
                output.Dispose();
            }
        }


        public static Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D newTexture = new Texture2D(targetWidth, targetHeight, source.format, false);
            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);
            for (int i = 0; i < newTexture.height; ++i)
            {
                for (int j = 0; j < newTexture.width; ++j)
                {
                    newTexture.SetPixel(j, i, source.GetPixelBilinear(incX * ((float)j), incY * ((float)i)));
                }
            }
            newTexture.Apply();
            return newTexture;
        }


        // NOTE: Leaving it for debuggin. Delete it as soon as it turns out to work properly.
        //public static Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight, GameObject Imagedebug)
        //{
        //    Debug.Log("Before Wdith: " + source.width);
        //    Debug.Log("Before Height: " + source.height);

        //    Texture2D newTexture = new Texture2D(targetWidth, targetHeight, source.format, false);
        //    float incX = (1.0f / (float)targetWidth);
        //    float incY = (1.0f / (float)targetHeight);
        //    for (int i = 0; i < newTexture.height; ++i)
        //    {
        //        for (int j = 0; j < newTexture.width; ++j)
        //        {
        //            newTexture.SetPixel(j, i, source.GetPixelBilinear(incX * ((float)j), incY * ((float)i)));
        //        }
        //    }
        //    newTexture.Apply();

        //    Debug.Log("After Wdith: " + newTexture.width);
        //    Debug.Log("After Height: " + newTexture.height);

        //    Imagedebug.GetComponent<Renderer>().material.mainTexture = newTexture;

        //    return newTexture;
        //}
    }
}
