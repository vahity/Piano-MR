using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using Unity.Collections;

public class ARCameraFeedToEyes : MonoBehaviour
{
    public RawImage leftEyeView;
    public RawImage rightEyeView;

    private ARCameraManager arCameraManager;
    private Texture2D cameraTexture;

    void Start()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();

        if (arCameraManager != null)
        {
            arCameraManager.frameReceived += OnCameraFrameReceived;
        }
        else
        {
            Debug.LogError("ARCameraManager not found in the scene.");
        }
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return;

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

        var rawTextureData = new NativeArray<byte>(image.GetConvertedDataSize(conversionParams), Allocator.Temp);
        image.Convert(conversionParams, rawTextureData);
        cameraTexture.LoadRawTextureData(rawTextureData);
        cameraTexture.Apply();
        rawTextureData.Dispose();
        image.Dispose();

        if (leftEyeView != null) leftEyeView.texture = cameraTexture;
        if (rightEyeView != null) rightEyeView.texture = cameraTexture;
    }
}
