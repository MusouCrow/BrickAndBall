using UnityEngine;
using System.Collections;

namespace Game {
    public static class Vibrate {
        private static AndroidJavaClass JC;
        private static AndroidJavaObject JO;
        private static bool HAS_INITED = false;

        public static void Init() {
            if (Application.platform == RuntimePlatform.Android && !HAS_INITED) {
                JC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                JO = JC.GetStatic<AndroidJavaObject>("currentActivity");
                HAS_INITED = true;
            }
        }

        public static void Do(int ms) {
            Vibrate.Init();

            if (Application.platform == RuntimePlatform.Android) {
                JO.CallStatic("Vibrate", ms.ToString());
            }    
        }
    }
}