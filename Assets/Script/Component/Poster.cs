using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Component {
	using Utility;

	public class Poster : MonoBehaviour {
		protected static GameObject MIRAGE;

		public AudioClip[] clips;
		public Vector3 targetScale;
		public float time;
		public float mirageArrivedPercent;

		private Timer timer;
		private bool hasMirage = false;
		private Vector3 originScale;

		protected void Awake () {
			if (MIRAGE == null) {
				MIRAGE = Resources.Load ("Prefab/Mirage") as GameObject;
			}

			this.originScale = this.transform.localScale;

			this.timer = new Timer ();
			this.timer.Enter (this.time);

			for (int i = 0; i < this.clips.Length; i++) {
				Sound.Play (this.clips [i]);
			}
		}

		void FixedUpdate () {
			this.timer.Update (Time.fixedDeltaTime);
			float process = this.timer.GetProcess ();
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, this.targetScale, process);

			if (process >= this.mirageArrivedPercent && !this.hasMirage) {
				GameObject obj = GameObject.Instantiate (MIRAGE, this.transform.position, this.transform.rotation, this.transform.parent) as GameObject;
				obj.transform.localScale = this.transform.localScale;
				obj.GetComponent<Mirage> ().Init (this.GetComponent<Image> ().sprite, this.originScale, this.time * (1 - this.mirageArrivedPercent));
				this.hasMirage = true;
			}

			if (!this.timer.IsRunning ()) {
				Destroy (this.gameObject);
			}
		}
	}
}