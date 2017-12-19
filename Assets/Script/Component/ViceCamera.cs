using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class ViceCamera : MonoBehaviour {
		private static ViceCamera VICE_CAMERA;
		
		public static void Shake(Vector3 power, float time) {
			VICE_CAMERA._Shake (power, time);
		}

		public static void Shake(Vector4 value) {
			VICE_CAMERA._Shake (value, value.w);
		}

		private Shaking shaking;
		private Vector3 position;

		// Use this for initialization
		void Awake () {
			VICE_CAMERA = this;
			this.shaking = new Shaking ();
		}
		
		// Update is called once per frame
		void Update () {
			if (this.shaking.IsRunning ()) {
				this.shaking.Update (Time.fixedDeltaTime);
				this.transform.localPosition = this.position + this.shaking.GetPosition ();
			}
		}

		public void _Shake (Vector3 power, float time) {
			this.shaking.Enter (power, time);
			this.position = this.transform.localPosition;
		}
	}
}