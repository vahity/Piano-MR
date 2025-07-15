using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections.LowLevel.Unsafe;

public class ManualStereoAR : MonoBehaviour
{
    public RawImage leftEyeImage;
    public RawImage rightEyeImage;

    private ARCameraManager arCameraManager;
    private Texture2D cameraTexture;
    private bool textureNeedsApply = false;

    void Awake()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager == null)
        {
            Debug.LogError("ARCameraManager ?? ???? ???? ???.");
            enabled = false;
        }
    }

    void OnEnable()
    {
        if (arCameraManager != null)
        {
            arCameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    void OnDisable()
    {
        if (arCameraManager != null)
        {
            arCameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    void Update()
    {
        if (textureNeedsApply)
        {
            // ????? ???? ??? GPU ?? thread ????
            cameraTexture.Apply();
            textureNeedsApply = false;
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
            transformation = XRCpuImage.Transformation.MirrorX | XRCpuImage.Transformation.MirrorY
    };

        if (cameraTexture == null || cameraTexture.width != image.width || cameraTexture.height != image.height)
        {
            cameraTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
            cameraTexture.filterMode = FilterMode.Bilinear; // ??? ?? ?? ????? ????

            leftEyeImage.texture = cameraTexture;
            rightEyeImage.texture = cameraTexture;
        }

        unsafe
        {
            image.Convert(conversionParams, new System.IntPtr(cameraTexture.GetRawTextureData<byte>().GetUnsafePtr()), cameraTexture.GetRawTextureData<byte>().Length);
        }

        image.Dispose();

        textureNeedsApply = true;
    }
}