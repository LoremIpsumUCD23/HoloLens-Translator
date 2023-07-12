using Config;
using ObjectDetection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using Microsoft.MixedReality.Toolkit.Utilities;

using Unity.Barracuda;

public class SceneAnalyzer : MonoBehaviour
{
    public CaptionController CaptionController;
    public TextMeshPro DebugText;
    public GameObject DebugQuad;

    private PhotoCapture photoCaptureObject = null;
    private IObjectDetectorClient _objectDetectorClient;

    // Data for scene analysis:
    private Texture2D image;
    private Matrix4x4 worldMatrix;
    private Matrix4x4 projectionMatrix;
    private Vector3 cameraPosition;

    public NNModel modelAsset;

    // Start is called before the first frame update
    void Start()
    {
        //this._objectDetectorClient = new AzureObjectDetector(Secrets.GetAzureImageRecognitionKey(), "https://obj-holo.cognitiveservices.azure.com/vision/v3.2/detect?model-version=latest");
        this._objectDetectorClient = new LocalObjDetection(modelAsset, 416, 416);
        CaptionController = GetComponent<CaptionController>();
    }

    /// <summary>
    /// The entry point for starting an analysis. This is called from the scene to begin taking a picture
    /// </summary>
    /// Camera code largely sourced from: https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/locatable-camera-in-unity
    /// There might be an issue with camera capture occurring on main thread, and not asynchronously at all.
    public void StartCapture()
    {
        DebugText.text = "Beginning screenshot process";
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    /// <summary>
    /// Initialize our photo capture object
    /// </summary>
    /// <param name="captureObject">The photo capture data</param>
    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        DebugText.text = "Photo Created";
        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);

    }
    /// <summary>
    /// Handle photo capture
    /// </summary>
    /// <param name="result">Whether the capture initialized successfully</param>
    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            DebugText.text = "Unable to start photo mode!";
        }
    }

    /// <summary>
    /// Function that handles what to do with captured photo data
    /// </summary>
    /// <param name="result">The result of the photo capture operation</param>
    /// <param name="photoCaptureFrame">The captured photo data</param>
    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {

            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            this.image = targetTexture;
            if (DebugQuad)
            {
                DebugQuad.GetComponent<Renderer>().material.mainTexture = targetTexture;
            }

            // Grab location, projection, and world information from camera at the time of capture
            Camera camera = CameraCache.Main;
            this.worldMatrix = camera.cameraToWorldMatrix;
            this.projectionMatrix = camera.projectionMatrix;
            this.cameraPosition = camera.transform.position;

            StartCoroutine(AnalyzeImage());
        }
        else
        {
            DebugText.text = "Failed to save photo to memory";
        }
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    /// <summary>
    /// Cleans up the photo capture object
    /// </summary>
    /// <param name="result">Result of the photo capture operation</param>
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    /// <summary>
    /// Coroutine to analyze an image. This sends the image off to our image recognition service and awaits the result
    /// </summary>
    /// <returns>IEnumerator waits on image processing result from Azure</returns>
    public IEnumerator AnalyzeImage()
    {
        DebugText.text = "Analyzing Image";
        yield return this._objectDetectorClient.DetectObjects(image, this.ProcessAnalysis);
    }

    /// <summary>
    /// Callback from image processing handles locating detected objects and creating caption objects for them
    /// </summary>
    /// <param name="detectedObjects">List of objects detected by the image recognition service</param>
    private void ProcessAnalysis(List<DetectedObject> detectedObjects)
    {
        DebugText.text = "Processing image analysis. Found " + detectedObjects.Count + " objects";
        // Clear previously created captions (We'll decide how to handle this better later)
        CaptionController.ClearCaptions();
        foreach (DetectedObject detectedObject in detectedObjects)
        {

            // Get average position of object's bounding rectangle
            Vector2 averagePos = new Vector2(detectedObject.rectangle.x + detectedObject.rectangle.w / 2,
                Screen.height - detectedObject.rectangle.y - detectedObject.rectangle.h / 2);

            // Cast to find location of object in 3D space
            RaycastHit hit;
            Ray ray = ScreenToWorldRay(averagePos, worldMatrix, projectionMatrix.inverse, cameraPosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 60f);
            Physics.Raycast(ray, out hit);

            Vector3 targetLocation = ray.origin + ray.direction;

            // Create caption at that location
            CaptionController.CreateCaption(detectedObject.objectName + ": " + detectedObject.confidence, targetLocation);
        }
    }

    /// <summary>
    /// This helper function takes a screen point and returns a ray cast from the saved camera matrix.
    /// </summary>
    /// <param name="screenPosition">2D coordinates to cast from</param>
    /// <param name="cameraWorld">Camera World Matrix</param>
    /// <param name="cameraProjectionInverse">Inverse of Camera Projection Matrix</param>
    /// <param name="cameraOrigin">Location of Camera</param>
    /// <returns>A ray traced from input screenPosition to 3D space determined by camera data inputs</returns>
    /// <source>https://forum.unity.com/threads/help-reproducing-screen-to-world-conversion.661558/</source>
    public Ray ScreenToWorldRay(Vector2 screenPosition, Matrix4x4 cameraWorld, Matrix4x4 cameraProjectionInverse, Vector3 cameraOrigin)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        screenPosition = new Vector2(screenPosition.x, screenHeight - screenPosition.y);

        Vector4 clipSpace = new Vector4(((screenPosition.x * 2.0f) / screenWidth) - 1.0f, (1.0f - (2.0f * screenPosition.y) / screenHeight), 0.0f, 1.0f);

        Vector4 viewSpace = cameraProjectionInverse * clipSpace;
        viewSpace /= viewSpace.w;

        Vector4 worldSpace = cameraWorld * viewSpace;

        Vector4 modifiedWorldSpace = worldSpace;
        modifiedWorldSpace.x -= cameraOrigin.x;
        modifiedWorldSpace.y -= cameraOrigin.y;
        modifiedWorldSpace.z -= cameraOrigin.z;

        Vector3 worldDirection = Vector3.Normalize(modifiedWorldSpace);

        return new Ray(cameraOrigin, worldDirection);
    }
}
