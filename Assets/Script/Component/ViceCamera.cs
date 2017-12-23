using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class ViceCamera : MonoBehaviour {
		private static ViceCamera instance;
		
		public static void Shake(Vector3 power, float time) {
			instance.shaking.Enter (power, time);
			instance.position = instance.transform.localPosition;
		}

		public static void Shake(Vector4 value) {
			ViceCamera.Shake (value, value.w);
		}

		public Transform target;
		public float speed;

		private Shaking shaking;
		private Vector3 position;

		// Use this for initialization
		private void Awake () {
			ViceCamera.instance = this;
			this.shaking = new Shaking ();
		}
		
		// Update is called once per frame
		private void FixedUpdate () {
			if (this.target) {
				this.transform.RotateAround (this.target.position, Vector3.up, this.speed);
			}

			if (this.shaking.IsRunning ()) {
				this.shaking.Update (Time.fixedDeltaTime);
				this.transform.localPosition = this.position + this.shaking.GetPosition ();
			}
		}
	}
}