using AlephVault.Unity.QRCapture.Authoring.Behaviours;
using AlephVault.Unity.QRCapture.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.QRCapture
{
    namespace Samples
    {
        [RequireComponent(typeof(Canvas))]
        public class SampleDisplayUser : MonoBehaviour
        {
            [SerializeField]
            private QRDisplay displayPrefab;

            private Button displayButton;
            private InputField displayText;

            private void Awake()
            {
                displayButton = transform.Find("OpenDisplay").GetComponent<Button>();
                displayText = transform.Find("TextToRender").GetComponent<InputField>();
                displayButton.onClick.AddListener(CaptureButtonPressed);
            }

            private void OnDestroy()
            {
                displayButton.onClick.RemoveListener(CaptureButtonPressed);
            }

            private async void CaptureButtonPressed()
            {
                await DisplayUtils.SetAndShow(displayPrefab, displayText.text);
            }
        }
    }
}
