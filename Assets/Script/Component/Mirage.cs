using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Utility {
	using Utility;

	public class Mirage : MonoBehaviour {
		public Vector3 targetScale;
		public float time;

		private Image image;
		private Timer timer;
		private Color targetColor;

		public void Init(Sprite sprite, Vector3 targetScale, float time) {
			this.image.sprite = sprite;
			this.image.SetNativeSize ();
			this.targetScale = targetScale;
			this.time = time;
			this.Ready ();
		}

		private void Ready() {
			this.timer.Enter (this.time);
			this.targetColor = this.image.color;
			this.targetColor.a = 0;
		}

		void Awake () {
			this.image = this.GetComponent<Image> ();
			this.timer = new Timer ();
		}

		void Start () {
			if (this.image.sprite != null) {
				this.Ready ();

			}
		}

		void FixedUpdate () {
			if (!this.timer.IsRunning ()) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, this.targetScale, this.timer.value);
			this.image.color = Color.Lerp (this.image.color, this.targetColor, this.timer.value);

			if (!this.timer.IsRunning ()) {
				//Destroy (this.gameObject);
			}
		}
	}
}