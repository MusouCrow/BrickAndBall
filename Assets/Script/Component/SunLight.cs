using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	using Utility;

	public class SunLight : MonoBehaviour {
		public ValueRound valueRound = new ValueRound ();

		new private Transform transform;
		private Vector3 eulerAngles;

		void Start () {
			this.transform = this.GetComponent<Transform> ();
			this.eulerAngles = this.transform.localEulerAngles;
			this.eulerAngles.x = this.valueRound.min;
			this.transform.localEulerAngles = this.eulerAngles;
		}

		void FixedUpdate () {
			this.valueRound.Update ();
			this.eulerAngles.x = this.valueRound.value;
			this.transform.localEulerAngles = this.eulerAngles;
		}

	}
}