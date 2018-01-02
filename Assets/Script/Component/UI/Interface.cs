using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component.UI {
	using Utility;

	public class Interface : MonoBehaviour {
		private static Interface INSTANCE;

		public static void Clear(float time=0) {
			INSTANCE.eventSystem.SetActive (false);
			INSTANCE.timer.Enter (time);
		}

		[SerializeField]
		private GameObject count;
		[SerializeField]
		private GameObject logo;
		[SerializeField]
		private GameObject eventSystem;

		private Timer timer;

		protected void Awake () {
			INSTANCE = this;

			this.timer = new Timer ();
			ViceCamera.OnEndEvent += OnCameraEnd;

			GameObject.Instantiate (this.logo, this.transform);

		}

		protected void FixedUpdate () {
			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);

				if (!this.timer.IsRunning ()) {
					Transform transform = INSTANCE.transform;

					for (int i = 0; i < transform.childCount; i++) {
						Transform child = transform.GetChild (i);

						if (child.name != "FPS") {
							Destroy (child.gameObject);
						}
					}

					this.eventSystem.SetActive (true);
				}
			}
		}

		private void OnCameraEnd(bool isGame) {
			if (isGame) {
				GameObject.Instantiate (this.count, this.transform);
			}
		}
	}
}