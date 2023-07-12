using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;


namespace ObjectDetection
{
    public class LocalObjDetection : IObjectDetectorClient
    {
        private Dictionary<string, int> models = new Dictionary<string, int>()
        {
            "mobilenetv2-10": 224
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
            Texture2D resized = LocalObjDetection.ScaleTexture(image, this.models.Get(this.modelName), this.models.Get(this.modelName));
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


        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            float sourceAspect = (float)source.width / source.height;
            float targetAspect = (float)targetWidth / targetHeight;
            int width = source.width;
            int height = source.height;
            if (sourceAspect < targetAspect)
            {
                // If source aspect is less than target aspect, scale width
                width = (int) (height * targetAspect);
            }
            else
            {
                // If source aspect is greater than target aspect, scale height
                height = (int) (width / targetAspect);
            }

            // Scale the original texture to the new dimensions
            TextureScale.Bilinear(source, width, height);

            // Create a new Texture2D of the target size and center the scaled image on it
            Texture2D result = new Texture2D(targetWidth, targetHeight);
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    // The color at each pixel will be that of the original texture if it falls within the scaled image region, or black otherwise
                    int sourceX = x - (width - targetWidth) / 2;
                    int sourceY = y - (height - targetHeight) / 2;
                    result.SetPixel(x, y, (sourceX >= 0 && sourceY >= 0 && sourceX < width && sourceY < height) ? source.GetPixel(sourceX, sourceY) : Color.black);
                }
            }
            result.Apply();
            return result;
        }
    }
}
