using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARCameraManager))]
public class CameraFeedToQuad : MonoBehaviour
{
    public Renderer quadRenderer;

    private ARCameraManager arCameraManager;
    private Texture2D cameraTexture;

    void Start()
    {
        arCameraManager = GetComponent<ARCameraManager>();
    }

    void OnEnable()
    {
        arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        Debug.Log("Camera frame received");
        if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            Debug.LogWarning("Failed to acquire CPU image");
            return;
        }

        Debug.Log("Got CPU image");
        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        if (cameraTexture == null || cameraTexture.width != image.width || cameraTexture.height != image.height)
        {
            cameraTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
        }

        var rawTextureData = cameraTexture.GetRawTextureData<byte>();
        image.Convert(conversionParams, rawTextureData);
        cameraTexture.Apply();

        quadRenderer.material.mainTexture = cameraTexture;

        image.Dispose();
    }
}
