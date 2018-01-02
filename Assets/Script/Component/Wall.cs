using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Wall : MonoBehaviour {
		[SerializeField]
		private Transform barTransform;
		public Vector4 shakingValue;

		private float target;
		private Timer timer;
		private Vector3 localScale;
		private Shaking shaking;
		private Vector3 position;

		void Awake() {
			this.timer = new Timer ();
			this.shaking = new Shaking ();
			this.position = this.transform.localPosition;
		}

		void FixedUpdate() {
			if (this.shaking.IsRunning ()) {
				this.shaking.Update (Time.fixedDeltaTime);
				this.transform.localPosition = this.position + this.shaking.GetPosition ();
			}

			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);
				this.localScale = this.barTransform.localScale;
				this.localScale.x = Mathf.Lerp (this.localScale.x, this.target, this.timer.value);
				this.barTransform.localScale = this.localScale;
			}
		}

		public void SetLength(float value) {
			this.target = value;
			this.timer.Enter (1);
		}

		void OnCollisionEnter(Collision collision) {
			this.shaking.Enter (this.shakingValue);
		}
	}
}