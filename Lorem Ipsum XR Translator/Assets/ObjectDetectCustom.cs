// using System.Diagnostics;
using Python.Runtime;
using UnityEngine;
using TMPro;
// using DebugSystem = System.Diagnostics.Debug;


public class Program
{
    
    public static void Main()
    {
        string pythonScriptPath = "Open-Detection-OpenCV/object_detection_image.py";
        string pythonExecutablePath = "python"; // Replace with the path to your Python executable if necessary
        ObjectDetectionRunner.RunObjectDetection(pythonScriptPath, pythonExecutablePath);
    }
}


public class ObjectDetectionCustom : MonoBehaviour
{
    // public TextMeshPro objCategory;
    public TextMeshPro textMesh;

    // Path to the Python executable 
    public string pythonExecutablePath;

    // Method to run the Python script
    private void RunObjectDetection(string pythonScriptPath)
    {
        using (Py.GIL())
        {
            dynamic detectionScript = Py.Import("Open-Detection-OpenCV/object_detection_image.py");
            dynamic result = detectionScript.ObjectDetection(pythonScriptPath); // Call the object detection function with the image path

            // Extract relevant information from the result
            dynamic boundingBoxes = result.bounding_boxes; // Assuming the result contains bounding box coordinates
            dynamic labels = result.labels; // Assuming the result contains object labels
            dynamic scores = result.scores; // Assuming the result contains confidence scores

            // Iterate over the detected objects
            for (int i = 0; i < boundingBoxes.Length(); i++)
            {
                dynamic boundingBox = boundingBoxes[i];
                dynamic label = labels[i];
                dynamic score = scores[i];

                //TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
                textMesh.text = result.objectName + ": " + result.confidence;

                // Process the detected object information as needed
                Debug.Log("Detected Object: " + label.ToString() + " (Score: " + score.ToString() + ")");
                Debug.Log("Bounding Box: X=" + boundingBox.x.ToString() + ", Y=" + boundingBox.y.ToString() + ", Width=" + boundingBox.width.ToString() + ", Height=" + boundingBox.height.ToString());
                
                Debug.Log(textMesh.text);
            }
        }
    }
}

public class ObjectDetectionRunner
{
    public static void RunObjectDetection(string pythonScriptPath, string pythonExecutablePath)
    {
        // Create a new process
        System.Diagnostics.Process process = new System.Diagnostics.Process();

        // Set the process properties
        process.StartInfo.FileName = pythonExecutablePath; // Path to the Python executable
        process.StartInfo.Arguments = pythonScriptPath; // Python script path as command-line argument
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;

        // Start the process
        process.Start();

        // Read the output
        string output = process.StandardOutput.ReadToEnd();

        // Wait for the process to finish
        process.WaitForExit();

    
        // Print the output
        Debug.Log(output);
        
    }
}














