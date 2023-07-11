using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

namespace ObjectDetection
{
    public class ImageDetectionBarracuda : MonoBehaviour
    {
        public NNModel modelAsset;
        private Model _runtimeModel;
        private IWorker _worker;

        // Start is called before the first frame update
        void Start()
        {
            _runtimeModel = ModelLoader.Load(modelAsset);
            _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, _runtimeModel);

            using (var tensor = new Tensor(1, 416, 416, 3))
            {
                _worker.Execute(tensor);
                var output = _worker.PeekOutput();

                // Get the class probabilities from the output tensor
                float[] classProbabilities = output.AsFloats();

                // Find the class with the highest probability
                int bestClass = 0;
                for (int i = 1; i < classProbabilities.Length; i++)
                {
                    if (classProbabilities[i] > classProbabilities[bestClass])
                    {
                        bestClass = i;
                        // Process the output here
                    }
                }
                Debug.Log(bestClass);
                Debug.Log("^ Best Class");
                
                output.Dispose();
            }
            Debug.Log("No problem");
        }


        private void OnDestroy()
        {
            _worker.Dispose();
        }
    }
}