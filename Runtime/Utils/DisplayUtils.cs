using System.Threading.Tasks;
using AlephVault.Unity.QRCapture.Authoring.Behaviours;
using AlephVault.Unity.Support.Utils;
using Object = UnityEngine.Object;

namespace AlephVault.Unity.QRCapture
{
    namespace Utils
    {
        /// <summary>
        ///   Utilities to display content to a QR code or a bar code,
        ///   or perhaps falling back to reading the clipboard.
        /// </summary>
        public static class DisplayUtils
        {
            /// <summary>
            ///   Sets the text to render and shows the QR.
            /// </summary>
            /// <param name="prefab">The prefab to use</param>
            /// <param name="text">The text to use</param>
            public static async Task Display(QRDisplay prefab, string text)
            {
                if (!prefab) return;

                QRDisplay instance = Object.Instantiate(prefab);
                instance.SetAndShow(text);
                await Tasks.Blink();
                while (instance.isActiveAndEnabled)
                {
                    await Tasks.Blink();
                }
                Object.Destroy(instance.gameObject);
            }
        }
    }
}
