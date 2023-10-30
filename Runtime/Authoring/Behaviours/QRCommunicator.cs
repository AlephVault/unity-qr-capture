using System.Threading.Tasks;
using AlephVault.Unity.QRCapture.Utils;
using UnityEngine;

namespace AlephVault.Unity.QRCapture
{
    namespace Authoring
    {
        namespace Behaviours
        {
            /// <summary>
            ///   This component allows the user to dump and read
            ///   text into and from QR codes.
            /// </summary>
            public class QRCommunicator : MonoBehaviour
            {
                /// <summary>
                ///   The display prefab to use to show QR codes.
                /// </summary>
                [SerializeField]
                private QRDisplay displayPrefab;

                /// <summary>
                ///   The capture prefab (only for mobile devices).
                /// </summary>
                [SerializeField]
                private MobileQRCapture capturePrefab;

                /// <summary>
                ///   Performs a qr capture. See <see cref="CaptureUtils.Capture" />
                ///   for more details.
                /// </summary>
                /// <returns>The captured text</returns>
                public async Task<string> Capture()
                {
                    return await CaptureUtils.Capture(capturePrefab);
                }

                /// <summary>
                ///   Performs a qr display. See <see cref="DisplayUtils.Display" />
                ///   for more details.
                /// </summary>
                /// <param name="text">The text to display</param>
                public async Task Display(string text)
                {
                    await DisplayUtils.Display(displayPrefab, text);
                }
            }
        }
    }
}