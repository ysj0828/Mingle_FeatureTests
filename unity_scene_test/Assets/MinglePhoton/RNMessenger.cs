using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;


namespace com.unity.photon
{
    public class NativeAPI
    {
#if UNITY_IOS && !UNITY_EDITOR
  [DllImport("__Internal")]
  public static extern void sendMessageToMobileApp(string message);
#endif
    }

    public class RNMessenger : MonoBehaviour
    {
        static public void SendToRN(string message)
        {
            Debug.LogError(message);
            return;
            if (Application.platform == RuntimePlatform.Android)
            {
#if !UNITY_EDITOR
        using (AndroidJavaClass jc = new AndroidJavaClass("com.azesmwayreactnativeunity.ReactNativeUnityViewManager"))
        {
          jc.CallStatic("sendMessageToMobileApp", message);
        }
#endif
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IOS && !UNITY_EDITOR
	        NativeAPI.sendMessageToMobileApp(message);
#endif
            }
        }
    }
}

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
// using UnityEngine.UI;
// using UnityEngine;

// public class NativeAPI {
// #if UNITY_IOS && !UNITY_EDITOR
//   [DllImport("__Internal")]
//   public static extern void sendMessageToMobileApp(string message);
// #endif
// }

// public class RNMessenger : MonoBehaviour
// {
//   static public void SendToRN(string message)
//   {
//     if (Application.platform == RuntimePlatform.Android)
//     {
//       using (AndroidJavaClass jc = new AndroidJavaClass("com.azesmwayreactnativeunity.ReactNativeUnityViewManager"))
//       {
//         jc.CallStatic("sendMessageToMobileApp", "The button has been tapped!");
//       }
//     }
//     else if (Application.platform == RuntimePlatform.IPhonePlayer)
//     {
// #if UNITY_IOS && !UNITY_EDITOR
//       NativeAPI.sendMessageToMobileApp("The button has been tapped!");
// #endif
//     }
//   }
// }
