using System;
using System.Collections;
using System.Collections.Generic;
using AlephVault.Unity.QRCapture.Authoring.Behaviours;
using AlephVault.Unity.QRCapture.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.QRCapture
{
    namespace Samples
    {
        [RequireComponent(typeof(Canvas))]
        public class SampleCaptureUser : MonoBehaviour
        {
            [SerializeField]
            private MobileQRCapture capturePrefab;

            private Button captureButton;
            private Text capturedText;

            private void Awake()
            {
                captureButton = transform.Find("Capture").GetComponent<Button>();
                capturedText = transform.Find("Text").GetComponent<Text>();
                captureButton.onClick.AddListener(CaptureButtonPressed);
            }

            private void OnDestroy()
            {
                captureButton.onClick.RemoveListener(CaptureButtonPressed);
            }

            private async void CaptureButtonPressed()
            {
                capturedText.text = await CaptureUtils.Capture(capturePrefab) ?? "Nothing yet";
            }
        }
    }
}
