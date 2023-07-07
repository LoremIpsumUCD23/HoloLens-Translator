using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imagePath))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to download image: " + www.error);
                    yield break;
                }

                // Get the downloaded texture
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                // Convert the texture to a byte array
                byte[] imageBytes = texture.EncodeToPNG();

                // Save the byte array as a temporary image file
                string tempImagePath = Application.persistentDataPath + "/temp_image.png";
                File.WriteAllBytes(tempImagePath, imageBytes);

                // Execute a Python script
                using (Py.GIL()) // Acquire the Global Interpreter Lock (GIL) for thread safety
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(modelPath); // Add the path to the Python script

                    dynamic script = Py.Import("object_detection_image");
                    dynamic globals = Py.CreateScope();
                    globals.image_path = tempImagePath;
                    PythonEngine.Exec(script, globals);
                    dynamic responseText = globals.response_text;
                    //script.execute();
                    //script.image_path = tempImagePath;
                    //script.execute();

                    //dynamic responseText = script.response_text;

                    Debug.Log(responseText);
                    callback(responseText);
                }

                // Delete the temporary image file
                File.Delete(tempImagePath);
            }
            // Shutdown Python engine
            PythonEngine.Shutdown();
        }
    }
}