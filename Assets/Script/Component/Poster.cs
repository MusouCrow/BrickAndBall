using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Game.Component {
	using Utility;

	public class Poster : MonoBehaviour {
		public delegate void OnEndDelegate();
		protected static GameObject MIRAGE;

		public AudioClip[] clips;
		public Vector3 targetScale;
		public Vector3 mirageScale;
		public float time;
		public float mirageArrivedPercent;
		public GameObject next;
		public bool willDestroy = true;
		public Vector4 shakingValue;
		public OnEndDelegate OnEndEvent;

		private Timer timer;
		private bool hasMirage = false;
		private Vector3 position;
		private Shaking shaking;

		protected void Awake () {
			if (MIRAGE == null) {
				MIRAGE = Resources.Load ("Prefab/UI/Mirage") as GameObject;
			}

			this.position = this.transform.localPosition;

			this.timer = new Timer ();
			this.timer.Enter (this.time);

			for (int i = 0; i < this.clips.Length; i++) {
				Sound.Play (this.clips [i]);
			}

			if (this.shakingValue.w > 0) {
				this.shaking = new Shaking ();
				this.shaking.Enter (this.shakingValue);
			}
		}

		void FixedUpdate () {
			if (this.shaking != null && this.shaking.IsRunning ()) {
				this.shaking.Update (Time.fixedDeltaTime);
				this.transform.localPosition = this.position + this.shaking.GetPosition ();
			}

			if (!this.timer.IsRunning ()) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);
			float process = this.timer.GetProcess ();
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, this.targetScale, process);

			if (process >= this.mirageArrivedPercent && !this.hasMirage) {
				GameObject obj = GameObject.Instantiate (MIRAGE, this.transform.parent) as GameObject;
				obj.transform.localPosition = this.transform.localPosition;
				obj.transform.localScale = this.transform.localScale;
				obj.transform.localRotation = this.transform.localRotation;

				obj.GetComponent<Mirage> ().Init (this.GetComponent<Image> ().sprite, this.mirageScale, this.time * (1 - this.mirageArrivedPercent));
				this.hasMirage = true;
			}

			if (!this.timer.IsRunning ()) {
				if (this.next != null) {
					GameObject.Instantiate (this.next, this.transform.parent);
				}

				if (this.willDestroy) {
					Destroy (this.gameObject);
				}

				if (this.OnEndEvent != null) {
					this.OnEndEvent ();
				}
			}
		}
	}
}