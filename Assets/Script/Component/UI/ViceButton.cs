using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Component.UI {
	using Utility;

	public class ViceButton : MonoBehaviour {
		private static AudioClip CLIP;
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
			if (CLIP == null) {
				CLIP = Resources.Load ("Sound/Click") as AudioClip;
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
			Sound.Play (CLIP);
			Mirage.New<Image> (this.transform, this.transform.parent, this.image, MIRAGE_SCALE, 0.5f);
			//Mirage.New<Text> (this.transform, this.transform.parent, this.text, MIRAGE_SCALE, 0.5f);
		}
	}
}