using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class Wall : MonoBehaviour {
		public Transform barTransform;
		private float target;
		private Timer timer;
		private Vector3 localScale;
		private Shaking shaking;

		void Awake() {
			this.timer = new Timer ();
			this.shaking = new Shaking (this.transform);
		}

		void FixedUpdate() {
			this.shaking.Update (Time.fixedDeltaTime);

			if (this.timer.isRunning) {
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
			this.shaking.Enter (2, 0.05f, 0.3f);
		}
	}
}