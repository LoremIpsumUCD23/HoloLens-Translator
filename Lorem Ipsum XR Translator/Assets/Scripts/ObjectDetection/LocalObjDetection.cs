using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;


namespace ObjectDetection
{
    public class LocalObjDetection : IObjectDetectorClient
    {
        private Dictionary<string, int> models = new Dictionary<string, int>()
        {
            {  "mobilenetv2-10", 224 }
        };
        private string modelName;
        private IWorker _worker;

        public LocalObjDetection(string modelName)
        {
            if (!models.ContainsKey(modelName)) throw new ArgumentException("Model \"" + modelName + "\" is not supported.");
            this.modelName = modelName;

            var runtimeModel = ModelLoader.Load("Assets/Scripts/ObjectDetection/Models/" + modelName + ".onnx");
            this._worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, runtimeModel);
        }


        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<List<DetectedObject>> callback)
        {
            yield break;
        }


        // TODO: Use something else instead of DetectedObject because it's Azure service specific
        public IEnumerator DetectObjects(string modelPath, Texture2D image, Action<List<DetectedObject>> callback)
        {
            if (this.models.TryGetValue(this.modelName, out int targetSize))
            {
                Texture2D resized = LocalObjDetection.ResizeTexture(image, targetSize, targetSize);
                using (var tensor = new Tensor(resized, 3))
                {
                    yield return null;

                    // Execute the model with the input tensor
                    this._worker.Execute(tensor);

                    var output = this._worker.PeekOutput();

                    // TODO: Process the output and prep a list of DetectedObjects to pass to callback()
                    Debug.Log(output);
                    List<DetectedObject> detectedObjects = new List<DetectedObject>();

                    callback(detectedObjects);

                    output.Dispose();
                }
            }
            else
            {
                Debug.Log("Image detection failed");
            }
        }


        public static Texture2D ResizeTexture(Texture2D sourceTexture, int targetWidth, int targetHeight)
        {
            // Create a new RenderTexture with the target dimensions
            RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 0);
            rt.antiAliasing = 8; // Adjust the anti-aliasing level as needed

            // Create a temporary camera to render the texture
            Camera tempCamera = new Camera();
            tempCamera.targetTexture = rt;
            tempCamera.Render();

            // Set the target texture as the active RenderTexture
            RenderTexture.active = rt;

            // Create a new Texture2D and read pixels from the active RenderTexture
            Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight);
            resizedTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
            resizedTexture.Apply();

            // Clean up temporary objects
            RenderTexture.active = null;

            return resizedTexture;
        }
    }
}
