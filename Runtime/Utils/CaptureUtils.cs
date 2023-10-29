using System;
using System.Threading.Tasks;
using AlephVault.Unity.QRCapture.Authoring.Behaviours;
using AlephVault.Unity.Support.Utils;
using UnityEngine;
using UnityEngine.Android;
using Object = UnityEngine.Object;

namespace AlephVault.Unity.QRCapture
{
    namespace Utils
    {
        /// <summary>
        ///   Utilities to capture content from a QR code or a bar code,
        ///   or perhaps falling back to reading the clipboard.
        /// </summary>
        public static class CaptureUtils
        {
            /// <summary>
            ///   Tells the authorization process was cancelled.
            /// </summary>
            public class AuthorizationCancelledException : Exception {}

            /// <summary>
            ///   Performs a capture using either the clipboard or a prefab.
            ///   Only mobile devices make use of the prefab. Also, consoles
            ///   do NOT have a webcam, so they always return null here.
            /// </summary>
            /// <param name="prefab">The prefab to use</param>
            /// <returns>The obtained capture</returns>
            public static async Task<string> Capture(MobileQRCapture prefab = null)
            {
                if (Application.isConsolePlatform)
                {
                    return null;
                }

                if (Application.isMobilePlatform && prefab)
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        // Check a first time whether it has authorization.
                        // If it does not, then ask for authorization. This
                        // uses Android-specific functions.
                        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                        {
                            Permission.RequestUserPermission(Permission.Camera);
                        }
                        // Then yes: check again for authorization. This time,
                        // if it does not, then abort. This uses Android-specific
                        // functions.
                        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                        {
                            throw new OperationCanceledException();
                        }
                    }
                    else
                    {
                        // Check a first time whether it has authorization.
                        // If it does not, then ask for authorization.
                        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                        {
                            await Application.RequestUserAuthorization(UserAuthorization.WebCam);
                        }
                        // Then yes: check again for authorization. This time,
                        // if it does not, then abort.
                        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                        {
                            throw new OperationCanceledException();
                        }
                    }

                    MobileQRCapture instance = null;
                    try
                    {
                        instance = Object.Instantiate(prefab);
                        // Let's wait **TWO** frames, explicitly.
                        await Tasks.Blink();
                        await Tasks.Blink();
                        // Capturing and then destroy.
                        return await instance.Capture();
                    }
                    finally
                    {
                        if (instance) Object.Destroy(instance.gameObject);
                    }
                }
                
                return GUIUtility.systemCopyBuffer;
            }
        }
    }
}
