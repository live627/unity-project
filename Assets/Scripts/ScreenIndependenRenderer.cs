using UnityEngine;

public class Supersampling : MonoBehaviour
{

    RenderTexture supersamplingRT;
    public Camera cam;
    Camera dummyCam;
    const int factor = 2;

    void Start()
    {
        dummyCam = gameObject.AddComponent<Camera>();
        dummyCam.cullingMask = 0;
        dummyCam.backgroundColor = Color.black;
        dummyCam.clearFlags = CameraClearFlags.SolidColor;
        supersamplingRT = new RenderTexture(Screen.width * factor, Screen.height * factor, 24, RenderTextureFormat.ARGB32);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        cam.targetTexture = supersamplingRT;
        cam.Render();
        cam.targetTexture = null;

        Graphics.Blit(supersamplingRT, destination);
    }
}

public class ScreenIndependenRenderer : MonoBehaviour
{
    RenderTexture viewPortRT;
    public float scale = 0.1f;
    private float lastScale;
    private Camera currentCamera;
    private Vector2 currentResolution;
    private bool shouldUpdateRenderTexture = false;

    Camera dummyCam;

    // How big or small the scale can be
    private float minScale = 0.1f;
    private float maxScale = 4.0f;

    void Awake()
    {
        // Get the current resolution
        currentResolution = new Vector2(Screen.width, Screen.height);

        // Set the last scale to the current scale to avoid creating multiple render textures on create
        lastScale = scale;

        // Set up the temporary camera
        dummyCam = gameObject.AddComponent<Camera>();
        dummyCam.cullingMask = 0;
        dummyCam.backgroundColor = Color.black;
        dummyCam.clearFlags = CameraClearFlags.SolidColor;

        // Create a render texture
        CreateRenderTexture();
    }

    void Update()
    {
        // Checks if the resolution has changed
        CheckResolution();

        // Checks if the camera has changed
        CheckCamera();

        // Checks if the scale has changed
        CheckScale();


        if (shouldUpdateRenderTexture)
        {
            CreateRenderTexture();
        }

        shouldUpdateRenderTexture = false;
    }

    // Checks for resolution changes
    void CheckResolution()
    {
        // Get the current screen resolution
        Vector2 tempRes = new Vector2(Screen.width, Screen.height);

        // Check if the resolution has changed
        if (tempRes != currentResolution)
        {
            // Save the new resolution
            currentResolution = tempRes;

            // Set the correct aspect ratio for the camera
            currentCamera.aspect = currentResolution.x / currentResolution.y;

            // Mark the render texture for updating
            shouldUpdateRenderTexture = true;
        }
    }

    void CheckCamera()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;

        // Check if there is a main camera
        if (mainCamera)
        {
            // Check if the main camera is the same as the current camera
            if (mainCamera != currentCamera)
            {
                // Camera is not the same, set the new camera
                currentCamera = mainCamera;

                // Set the correct aspect ratio for the camera
                currentCamera.aspect = currentResolution.x / currentResolution.y;
            }
        }
        else
        {
            // Set to null
            currentCamera = null;
        }
    }

    void CheckScale()
    {
        if (lastScale != scale && currentCamera)
        {
            // Check if the scale is below zero
            if (scale <= minScale)
            {
                scale = minScale;
            }

            // Check if the scale is above the max resolution
            else if (scale >= maxScale)
            {
                scale = maxScale;
            }

            // Log the scale
            lastScale = scale;

            // Mark the render texture for updating
            shouldUpdateRenderTexture = true;
        }
    }

    // Creates the rendertexture
    void CreateRenderTexture()
    {
        viewPortRT = new RenderTexture((int)(Screen.width * scale), (int)(Screen.height * scale), 24, RenderTextureFormat.ARGB32);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Check if we have a camera before rendering
        if (currentCamera && viewPortRT)
        {
            currentCamera.targetTexture = viewPortRT;
            currentCamera.Render();
            currentCamera.targetTexture = null;

            Graphics.Blit(viewPortRT, destination);
        }
    }
}


