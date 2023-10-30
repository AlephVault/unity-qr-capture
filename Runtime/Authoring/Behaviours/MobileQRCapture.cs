using System;
using System.Linq;
using System.Threading.Tasks;
using AlephVault.Unity.Support.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ZXing;

namespace AlephVault.Unity.QRCapture
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   Mobile devices will use this class to read
            ///   a QR code and return whatever text was read.
            /// </summary>
            [RequireComponent(typeof(CanvasScaler))]
            [RequireComponent(typeof(Canvas))]
            public class MobileQRCapture : MonoBehaviour
            {
                // The current canvas component.
                private Canvas canvas;

                // The current canvas scaler component.
                private CanvasScaler scaler;

                /// <summary>
                ///   The target of the captured image. It MUST be a direct child.
                /// </summary>
                [SerializeField]
                private RawImage captureTarget;

                /// <summary>
                ///   The resolution-side (i.e. {value}x{value}) of the
                ///   capture target texture.
                /// </summary>
                [SerializeField]
                private uint captureTargetResolution = 1000;

                /// <summary>
                ///   The (optional) scan button. Without scan button,
                ///   the scan will be attempted in each frame.
                /// </summary>
                [SerializeField]
                private Button scanButton;

                /// <summary>
                ///   The (optional, but recommended) cancel button.
                /// </summary>
                [SerializeField]
                private Button cancelButton;

                /// <summary>
                ///   The timeout for auto-cancellation. Specially
                ///   useful when no using cancel button.
                /// </summary>
                [SerializeField]
                private float cancelTimeout = 0;

                /// <summary>
                ///   The aspect mode to use in the image.
                /// </summary>
                [SerializeField]
                private AspectRatioFitter.AspectMode aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

                // The current webcam texture.
                private WebCamTexture webcamTexture;

                // Whether the current capture process is cancelled or not.
                private bool captureCancelled = false;

                private void Awake()
                {
                    SetupComponents();
                    SetupCameraTexture();
                }

                private void Start()
                {
                    if (cancelTimeout > 0)
                    {
                        AutoCancel();
                    }
                }

                private async void AutoCancel()
                {
                    float time = 0;
                    while (time < cancelTimeout && gameObject)
                    {
                        await Tasks.Blink();
                        Debug.Log($"Time is: {time}");
                        time += Time.unscaledDeltaTime;
                    }
                    captureCancelled = true;
                }

                private void SetupComponents()
                {
                    canvas = GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    scaler = GetComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    
                    if (scanButton && scanButton.transform.parent != transform)
                    {
                        Destroy(gameObject);
                        throw new InvalidOperationException(
                            "The scan button must be set to a direct child of this QR Scanner"
                        );
                    }

                    if (!captureTarget || captureTarget.transform.parent != transform)
                    {
                        Destroy(gameObject);
                        throw new InvalidOperationException(
                            "The capture target must be set to a direct child of this QR Scanner"
                        );
                    }
                    captureTarget.rectTransform.anchoredPosition = Vector2.zero;
                    captureTarget.rectTransform.anchorMin = Vector2.zero;
                    captureTarget.rectTransform.anchorMax = Vector2.one;
                    captureTarget.rectTransform.sizeDelta = Vector2.zero;
                    AspectRatioFitter captureTargetFitter = captureTarget.GetComponent<AspectRatioFitter>() ??
                                                            captureTarget.gameObject.AddComponent<AspectRatioFitter>();
                    captureTargetFitter.aspectRatio = 1;
                    captureTargetFitter.aspectMode = aspectMode;

                    if (cancelButton)
                    {
                        if (cancelButton.transform.parent != transform)
                        {
                            throw new InvalidOperationException(
                                "The cancel button must be set to a direct child of this QR Scanner"
                            );
                        }
                        cancelButton.onClick.AddListener(CancelCapture);
                    }
                }

                private void SetupCameraTexture()
                {
                    WebCamDevice device = (from aDevice in WebCamTexture.devices
                                           where !aDevice.isFrontFacing
                                           select aDevice).FirstOrDefault();
                    if (!device.Equals(default))
                    {
                        webcamTexture = new WebCamTexture(
                            device.name, (int)captureTargetResolution, (int)captureTargetResolution
                        );
                        webcamTexture.Play();
                        captureTarget.texture = webcamTexture;
                    }
                }
                
                private void Update()
                {
                    if (webcamTexture != default)
                    {
                        int orientation = -webcamTexture.videoRotationAngle;
                        captureTarget.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
                    }
                }

                private void OnDestroy()
                {
                    if (cancelButton)
                    {
                        cancelButton.onClick.RemoveListener(CancelCapture);
                    }
                    webcamTexture.Stop();
                }

                /// <summary>
                ///   Starts a QR capture, and gets its captured result.
                /// </summary>
                /// <returns>The captured result</returns>
                public async Task<string> Capture()
                {
                    IBarcodeReader reader = new BarcodeReader();
                    captureCancelled = false;
                    string text = null;

                    void Scan()
                    {
                        Result result = reader.Decode(
                            webcamTexture.GetPixels32(), 
                            webcamTexture.width, webcamTexture.height
                        );
                        text = result?.Text;
                    }

                    try
                    {
                        if (scanButton)
                        {
                            scanButton.onClick.AddListener(Scan);
                        }

                        while (!captureCancelled)
                        {
                            try
                            {
                                if (!scanButton) Scan();
                            }
                            catch (Exception)
                            {
                                // Ignoring this one explicitly.
                            }

                            if (text != null) return text;
                            await Tasks.Blink();
                        }
                    }
                    finally
                    {
                        if (scanButton)
                        {
                            scanButton.onClick.RemoveListener(Scan);
                        }
                    }

                    return null;
                }

                /// <summary>
                ///   Cancels a capture, if any.
                /// </summary>
                public void CancelCapture()
                {
                    captureCancelled = true;
                }
            }
        }
    }
}
