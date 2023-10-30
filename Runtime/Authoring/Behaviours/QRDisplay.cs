using System;
using System.Collections;
using System.Collections.Generic;
using QRCoder;
using QRCoder.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AlephVault.Unity.QRCapture
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   This display serves as a handler to show
            ///   a QR code in an image.
            /// </summary>
            [RequireComponent(typeof(CanvasScaler))]
            [RequireComponent(typeof(Canvas))]
            public class QRDisplay : MonoBehaviour
            {
                // The current canvas component.
                private Canvas canvas;

                // The current canvas scaler component.
                private CanvasScaler scaler;

                // The last display image set in this component.
                private Texture2D displayImage;

                // The current display text value.
                private string text = "";

                /// <summary>
                ///   The color used as dark when generating the QR.
                /// </summary>
                [SerializeField]
                public Color darkColor = Color.black;
                
                /// <summary>
                ///   The color used as light when generating the QR.
                /// </summary>
                [SerializeField]
                public Color lightColor = Color.white;
                
                /// <summary>
                ///   The target of the displayed image. It MUST be a direct child.
                /// </summary>
                [SerializeField]
                private RawImage displayTarget;
                
                /// <summary>
                ///   The (optional) close button. Without close button, the user
                ///   must manually implement their own mean to close, or set the
                ///   <see cref="closeOnClick" /> property to true. Otherwise, it
                ///   will never close.
                /// </summary>
                [SerializeField]
                private Button closeButton;
                
                private void Awake()
                {
                    canvas = GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    scaler = GetComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);

                    if (!displayTarget || displayTarget.transform.parent != transform)
                    {
                        Destroy(gameObject);
                        throw new InvalidOperationException(
                            "The display image must be set to a direct child of this QR Display"
                        );
                    }
                    displayTarget.rectTransform.anchoredPosition = Vector2.zero;
                    displayTarget.rectTransform.anchorMin = Vector2.zero;
                    displayTarget.rectTransform.anchorMax = Vector2.one;
                    displayTarget.rectTransform.sizeDelta = Vector2.zero;
                    AspectRatioFitter captureTargetFitter = displayTarget.GetComponent<AspectRatioFitter>() ??
                                                            displayTarget.gameObject.AddComponent<AspectRatioFitter>();
                    captureTargetFitter.aspectRatio = 1;
                    captureTargetFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    
                    if (closeButton)
                    {
                        Debug.Log("Setting close button");
                        if (closeButton.transform.parent != transform)
                        {
                            throw new InvalidOperationException(
                                "The close button must be set to a direct child of this QR Scanner"
                            );
                        }
                        closeButton.onClick.AddListener(() =>
                        {
                            gameObject.SetActive(false);
                            Debug.Log("Tu culo");
                        });
                    }
                }

                /// <summary>
                ///   Gets and sets the text. Null values are converted
                ///   to empty strings. Setting an empty string clears
                ///   the current image, if any. Setting otherwise will
                ///   generate a new image and clear the previous one,
                ///   if any.
                /// </summary>
                public string Text
                {
                    get => text;
                    set
                    {
                        if (text == value) return;
                        
                        if (displayImage)
                        {
                            Destroy(displayImage);
                            displayImage = null;
                        }

                        value ??= "";
                        if (!string.IsNullOrEmpty(value))
                        {
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                            UnityQRCode qrCode = new UnityQRCode(qrCodeData);
                            displayImage = qrCode.GetGraphic(20, darkColor, lightColor);
                            displayTarget.texture = displayImage;
                        }
                    }
                }

                /// <summary>
                ///   <see cref="darkColor"/>.
                /// </summary>
                public Color DarkColor
                {
                    get => darkColor;
                    set => darkColor = value;
                }
                
                /// <summary>
                ///   <see cref="lightColor"/>.
                /// </summary>
                public Color LightColor
                {
                    get => lightColor;
                    set => lightColor = value;
                }

                /// <summary>
                ///   Sets a QR text for the component and shows it.
                /// </summary>
                /// <param name="withText">The text to show</param>
                public void SetAndShow(string withText)
                {
                    Text = withText;
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
