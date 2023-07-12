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

                // TODO: Process the output and prep a list of DetectedObjects to pass to callback()
                List<DetectedObject> detectedObjects = new List<DetectedObject>();
                Debug.Log(output.shape);
                Debug.Log(output);
                // float threshold = 0.5f;
                // for (int i = 0; i < output.shape[2]; i++)
                // {
                //     float confidence = output[0, 0, i, 3];
                //     if (confidence > threshold)
                //     {
                //         int classLabel = (int)output[0, 0, i, 2];

                //         float xmin = output[0, 0, i, 4];
                //         float ymin = output[0, 0, i, 5];
                //         float xmax = output[0, 0, i, 6];
                //         float ymax = output[0, 0, i, 7];

                //         // The coordinates are normalized to [0, 1], you may need to scale them back to pixel space.
                //         // Let's assume the original image size is (imageWidth, imageHeight)
                //         Rectangle rect = new Rectangle(
                //             (int)(xmin * this._inputWidth),
                //             (int)(ymin * this._inputHeight),
                //             (int)((xmax - xmin) * this._inputWidth), 
                //             (int)((ymax - ymin) * this._inputHeight));

                //         string className = classLabel.ToString(); // Replace with actual class name if you have a dictionary mapping class labels to names.

                //         detectedObjects.Add(new DetectedObject(rect, className, confidence));
                //     }
                // }

                callback(detectedObjects);
                
                output.Dispose();
            }
        }


        // TODO: Fix resizing 
        public static Texture2D ResizeTexture(Texture2D sourceTexture, int targetWidth, int targetHeight, GameObject Imagedebug)
        {
            // Error handling: Check if source texture is null
            if (sourceTexture == null)
            {
                Debug.LogError("Source texture is null");
                return null;
            }
            
            // Create a copy of the source texture
            Texture2D copiedTexture = sourceTexture;
            Debug.Log(sourceTexture.Reinitialize ( targetWidth, targetHeight));
            sourceTexture.Apply();
            Imagedebug.GetComponent<Renderer>().material.mainTexture = sourceTexture;
        
            // Resize the copied texture
            // copiedTexture.Reinitialize(targetWidth, targetHeight);
            
        
            return sourceTexture;
        }

        public static Texture2D ResizeTexture(Texture2D sourceTexture, int targetWidth, int targetHeight)
        {
            // Error handling: Check if source texture is null
            if (sourceTexture == null)
            {
                Debug.LogError("Source texture is null");
                return null;
            }
            
            // Create a copy of the source texture
            Texture2D copiedTexture = UnityEngine.Object.Instantiate(sourceTexture) as Texture2D;
            copiedTexture.Reinitialize ( targetWidth, targetHeight);
           
        
            // Resize the copied texture
            // copiedTexture.Reinitialize(targetWidth, targetHeight);
            // copiedTexture.Apply();
        
            return copiedTexture;
        }
    }
    
}
