using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class WallLight : MonoBehaviour {
		public ValueRound valueRound = new ValueRound ();

		private Light light;

		void Start () {
			this.light = this.GetComponent<Light> ();
			this.light.intensity = this.valueRound.min;
		}

		void FixedUpdate () {
			this.valueRound.Update ();
			this.light.intensity = this.valueRound.value;
		}
	}
}