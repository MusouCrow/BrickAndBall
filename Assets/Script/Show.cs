using UnityEngine;

namespace Game {
    public class Show : MonoBehaviour {
        protected void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                ScreenCapture.CaptureScreenshot("Screen.png");
            }
        }
    }
}