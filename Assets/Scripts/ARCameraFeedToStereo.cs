using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARCameraFeedToStereo : MonoBehaviour
{
    public RawImage leftEyeView;
    public RawImage rightEyeView;

    private ARCameraManager arCameraManager;
    private Texture2D cameraTexture;

    void Start()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager != null)
            arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image)) return;

        var format = TextureFormat.RGBA32;

        if (cameraTexture == null || cameraTexture.width != image.width || cameraTexture.height != image.height)
        {
            cameraTexture = new Texture2D(image.width, image.height, format, false);
        }

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
           // outputFormat = XRCpuImage.Transformation.MirrorY,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        var rawTextureData = cameraTexture.GetRawTextureData<byte>();
        image.Convert(conversionParams, rawTextureData);
        cameraTexture.Apply();

        leftEyeView.texture = cameraTexture;
        rightEyeView.texture = cameraTexture;

        image.Dispose();
    }
}
