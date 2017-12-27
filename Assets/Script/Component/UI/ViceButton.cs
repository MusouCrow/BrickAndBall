using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Component.UI {
	using Utility;

	public class ViceButton : MonoBehaviour {
		private static AudioClip CLICK_CLIP;
		private static AudioClip SHOW_CLIP;
		private static Vector3 MIRAGE_SCALE = new Vector3 (3, 3, 3);
		private const float SHOW_TIME = 0.3f; 

		private static void SetAlpha (Graphic graphic, float value) {
			Color color = graphic.color;
			color.a = value;
			graphic.color = color;
		}

		public GameObject next;

		protected Button button;
		private Image image;
		private Text text;
		private Timer timer;

		protected void Awake () {
			if (CLICK_CLIP == null) {
				CLICK_CLIP = Resources.Load ("Sound/Click") as AudioClip;
			}

			if (SHOW_CLIP == null) {
				SHOW_CLIP = Resources.Load ("Sound/Count") as AudioClip;
			}

			this.image = this.GetComponent<Image> ();
			this.text = this.GetComponentInChildren<Text> ();

			ViceButton.SetAlpha (this.image, 0);
			ViceButton.SetAlpha (this.text, 0);

			this.button = this.GetComponent<Button> ();
			this.button.onClick.AddListener (this.PlaySound);
			this.button.enabled = false;

			this.timer = new Timer ();
			this.timer.Enter (SHOW_TIME);

			Sound.Play (SHOW_CLIP);
		}

		protected void FixedUpdate () {
			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);
				float process = this.timer.GetProcess ();

				ViceButton.SetAlpha (this.image, process);
				ViceButton.SetAlpha (this.text, process);

				if (!this.timer.IsRunning ()) {
					this.button.enabled = true;

					if (this.next != null) {
						GameObject.Instantiate (this.next, this.transform.parent);
					}
				}
			}
		}

		private void PlaySound () {
			Sound.Play (CLICK_CLIP);
			Mirage.New<Image> (this.transform, this.transform.parent, this.image, MIRAGE_SCALE, 0.5f);
			//Mirage.New<Text> (this.transform, this.transform.parent, this.text, MIRAGE_SCALE, 0.5f);
		}
	}
}