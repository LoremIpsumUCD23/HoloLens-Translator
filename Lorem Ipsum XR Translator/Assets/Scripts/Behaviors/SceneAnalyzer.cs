using Config;
using ObjectDetection;
using Feedback;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using Microsoft.MixedReality.Toolkit.Utilities;



public class SceneAnalyzer : MonoBehaviour
{
    public CaptionController CaptionController;
    public TextMeshPro DebugText;
    public GameObject DebugQuad;
    public ParticleSystem LoadingPS;
    public GameObject AnalyzeVisualFeedback;

    private PhotoCapture photoCaptureObject = null;
    private IObjectDetectorClient _objectDetectorClient;
    private IObjectDetectorClient _objectDetectorGCPClient;
    private string _detectionService = "unknown";
    private System.Random _rand;
    private double _probAzure = 1.0;

    // Feedback
    private FeedbackClient _feedbackClient;

    // Data for scene analysis:
    private Texture2D image;
    private Matrix4x4 worldMatrix;
    private Matrix4x4 projectionMatrix;
    private Vector3 cameraPosition;
    private float textureWidth;
    private float textureHeight;
    private bool analyzing = false;

    // Constants for camera offset and optical warp based on device testing
    private Vector3 CAMERA_OFFSET = new Vector3(0, 0.009f, 0.05f);
    private const float OPTICAL_WARP_FACTOR = 0.521f;
    private const float MAX_RAY_DIST = 3.1f;

    // Timer
    private System.Diagnostics.Stopwatch timer;
    private System.Diagnostics.Stopwatch apiTimer;

    // Start is called before the first frame update
    void Start()
    {
        CaptionController = GetComponent<CaptionController>();
        // For randomising Azure and GCP calls
        _rand = new System.Random();
        _objectDetectorClient = new AzureObjectDetector(Secrets.GetAzureImageRecognitionKey());
        _objectDetectorGCPClient = new GCPObjectDetection(Secrets.GetGCPCloudVisionKey());
        _feedbackClient = new FeedbackClient(Secrets.GetFeedbackServerEndpoint());
        timer = new System.Diagnostics.Stopwatch();
        apiTimer = new System.Diagnostics.Stopwatch();
    }

    /// <summary>
    /// The entry point for starting an analysis. This is called from the scene to begin taking a picture
    /// </summary>
    /// Camera code largely sourced from: https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/locatable-camera-in-unity
    /// There might be an issue with camera capture occurring on main thread, and not asynchronously at all.
    public void StartCapture()
    {
        // Don't allow multiple attempts at capturing until the capture process is complete.
        if (analyzing)
        {
            return;
        }

        analyzing = true;
        AnalyzeVisualFeedback.SetActive(true);
        DebugText.text = "Beginning screenshot process";

        Debug.Log("Timer starting..");
        timer.Start();

        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        if (LoadingPS != null)
        {
            LoadingPS.Play();
        }
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
            analyzing = false;
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            if (LoadingPS != null)
            {
                LoadingPS.Stop();
            }
        }
    }

    /// <summary>
    /// Function that handles what to do with captured photo data
    /// </summary>
    /// <param name="result">The result of the photo capture operation</param>
    /// <param name="photoCaptureFrame">The captured photo data</param>
    private async void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
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

            // Get virtual camera data
            Camera camera = CameraCache.Main;
            worldMatrix = camera.cameraToWorldMatrix;
            projectionMatrix = camera.projectionMatrix;
            cameraPosition = camera.transform.position;

           await AnalyzeImage();
        }
        else 
        { 
            DebugText.text = "Failed to save photo to memory";
            analyzing = false;
            if (LoadingPS != null)
            {
                LoadingPS.Stop();
            }
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
    public async Task AnalyzeImage()
    {
        textureWidth = image.width;
        textureHeight = image.height; 
        DebugText.text = "Analyzing Image";
        apiTimer.Start();

        if (this._rand.NextDouble() <= this._probAzure)
        {
            Debug.Log("Use Azure Object Detection");
            this._detectionService = "Azure";
            await this._objectDetectorClient.DetectObjects("https://obj-holo.cognitiveservices.azure.com/vision/v3.2/detect?model-version=latest",
            image, this.ProcessAnalysis);
        }
        else
        {
            Debug.Log("Use GCP Object Detection");
            this._detectionService = "GCP";
            await this._objectDetectorGCPClient.DetectObjects("", image, this.ProcessAnalysis);
        }
    }

    /// <summary>
    /// Callback from image processing handles locating detected objects and creating caption objects for them
    /// </summary>
    /// <param name="detectedObjects">List of objects detected by the image recognition service</param>
    private void ProcessAnalysis(List<DetectedObject> detectedObjects)
    {
        if (LoadingPS != null)
        {
            LoadingPS.Stop();
        }
        analyzing = false;
        DebugText.text = "Processing image analysis. Found " + detectedObjects.Count + " objects";
        // Clear previously created captions (We'll decide how to handle this better later)
        CaptionController.ClearCaptions();
        
        foreach (DetectedObject detectedObject in detectedObjects)
        {

            // Get average position of object's bounding rectangle
            Vector2 averagePos = new Vector2(detectedObject.rectangle.x + detectedObject.rectangle.w / 2,
                textureHeight - detectedObject.rectangle.y - detectedObject.rectangle.h / 2);
            // Find the center of our image
            Vector2 imageCenter = new Vector2(textureWidth / 2, textureHeight / 2);
            // Get the offset of our target position relative to the center of the image
            Vector2 centerOffset = averagePos - imageCenter;
            // Warp the target position by our constant warp factor
            Vector2 warpedPos = averagePos + (centerOffset * OPTICAL_WARP_FACTOR);

            // Cast to find location of object in 3D space (using optical warp, and adjusting camera by virtual offset)
            RaycastHit hit;
            Ray ray = ScreenToWorldRay(warpedPos, worldMatrix, projectionMatrix.inverse, cameraPosition + CAMERA_OFFSET);

            // Do not collide with other captions
            string[] layerNames = {"Captions", "UI" };
            LayerMask captionMask = LayerMask.GetMask(layerNames);
            Physics.Raycast(ray, out hit, MAX_RAY_DIST * 1.5f, ~captionMask);

            Vector3 targetLocation = ray.origin + ray.direction * MAX_RAY_DIST;
            if (hit.transform && hit.transform.tag != "caption")
            {
                targetLocation = hit.point;
            }

            // Create caption at that location
            CaptionController.CreateCaption(detectedObject.objectName + ": " + detectedObject.confidence, targetLocation);

            Debug.Log("API Call: " + apiTimer.ElapsedMilliseconds + " ms");
            Debug.Log("Total Elapsed: " + timer.ElapsedMilliseconds + " ms");
        }
        AnalyzeVisualFeedback.SetActive(false);
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
        screenPosition = new Vector2((screenPosition.x / textureWidth) * textureWidth, textureHeight - (screenPosition.y / textureHeight) * textureHeight);

        Vector4 clipSpace = new Vector4(((screenPosition.x * 2.0f) / textureWidth) - 1.0f, (1.0f - (2.0f * screenPosition.y) / textureHeight), 0.0f, 1.0f);

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

    public void PutGoodFeedbackForObjectDetection()
    {
        Debug.Log("Send a positive feedback to feedback server (object detection)");
        StartCoroutine(this._feedbackClient.PutFeedback("detection", this._detectionService, true));
    }

    public void PutBadFeedbackForObjectDetection()
    {
        Debug.Log("Send a negative feedback to feedback server (object detection)");
        StartCoroutine(this._feedbackClient.PutFeedback("detection", this._detectionService, false));
    }
}
