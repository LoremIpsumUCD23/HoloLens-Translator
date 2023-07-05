using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;
using System.IO;

namespace ObjectDetection
{
    public class CustomObjectDetector : IObjectDetectorClient
    {
        public CustomObjectDetector()
        {
            string pythonDLLPath = Application.dataPath + "/Plugins/PythonNet/python38.dll"; // Adjust the path accordingly
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDLLPath);
            //Environment.SetEnvironmentVariable("PATH", pythonDLLPath);
            PythonEngine.Initialize();
        }
        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<string> callback)
        {
            // Execute a Python script
            using (Py.GIL()) // Acquire the Global Interpreter Lock (GIL) for thread safety
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(modelPath); // Add the path to the Python script

                //string pythonScriptPath = Path.Combine(Application.dataPath, "Scripts/ObjectDetection/PythonCustomModel/object_detection_image.py");
                //pythonScriptPath = pythonScriptPath.Replace("\\", "/");

                dynamic script = Py.Import("object_detection_image");

                script.image_path = imagePath;
                script.execute();

                dynamic responseText = script.response_text;

                Debug.Log(responseText);
                callback(responseText);
            }

            // Shutdown Python engine
            PythonEngine.Shutdown();
            yield return null;
        }
    }
}