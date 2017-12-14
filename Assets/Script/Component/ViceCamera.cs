using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class ViceCamera : MonoBehaviour {
		public float time;

		private Shaking shaking;

		// Use this for initialization
		void Awake () {
			Lib.BindCamera (this);
			this.shaking = new Shaking (this.transform);
		}
		
		// Update is called once per frame
		void Update () {
			this.shaking.Update (Time.fixedDeltaTime);
		}

		public void Shake (Vector3 power, float time) {
			this.shaking.Enter (power, time);
		}
	}
}