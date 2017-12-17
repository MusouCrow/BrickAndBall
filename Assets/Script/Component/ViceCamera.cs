using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class ViceCamera : MonoBehaviour {
		public float time;

		private Shaking shaking;
		private Vector3 position;

		// Use this for initialization
		void Awake () {
			Lib.BindCamera (this);
			this.shaking = new Shaking ();
		}
		
		// Update is called once per frame
		void Update () {
			if (this.shaking.IsRunning ()) {
				this.shaking.Update (Time.fixedDeltaTime);
				this.transform.localPosition = this.position + this.shaking.GetPosition ();
			}
		}

		public void Shake (Vector3 power, float time) {
			this.shaking.Enter (power, time);
			this.position = this.transform.localPosition;
		}
	}
}