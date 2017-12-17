using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Component {
	using Utility;

	public class Count : MonoBehaviour {
		private static AudioClip CLIP;
		private static GameObject MIRAGE;
		private static Vector3 ORIGIN_SCALE = new Vector3 (5, 5, 5);
		private static Vector3 TARGET_SCALE = new Vector3 (2.5f, 2.5f, 2.5f);
		private static float SCALE_TIME = 0.8f;
		private static float MIRAGE_TIME = 0.5f;

		public AudioClip clip;

		private Timer timer;

		// Use this for initialization
		void Awake () {
			if (CLIP == null) {
				CLIP = Resources.Load ("Sound/Count") as AudioClip;
			}
			if (MIRAGE == null) {
				MIRAGE = Resources.Load ("Prefab/Mirage") as GameObject;
			}

			this.timer = new Timer ();
			this.timer.Enter (SCALE_TIME);

			AudioSource.PlayClipAtPoint (CLIP, Vector3.zero);
			AudioSource.PlayClipAtPoint (this.clip, Vector3.zero);
		}
		
		// Update is called once per frame
		void FixedUpdate () {
			this.timer.Update (Time.fixedDeltaTime);
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, TARGET_SCALE, this.timer.value);

			if (!this.timer.IsRunning ()) {
				Destroy (this.gameObject);
				GameObject obj = GameObject.Instantiate (MIRAGE, this.transform.position, this.transform.rotation, this.transform.parent) as GameObject;
				obj.transform.localScale = this.transform.localScale;
				obj.GetComponent<Mirage> ().Init (this.GetComponent<Image> ().sprite, ORIGIN_SCALE, MIRAGE_TIME);
			}
		}
	}
}