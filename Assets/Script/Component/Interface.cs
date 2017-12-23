using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Interface : MonoBehaviour {
		public GameObject count;
		public GameObject logo;

		void Start () {
			GameObject.Instantiate (this.logo, this.transform);
			//GameObject.Instantiate (this.count, this.transform);
		}

		void FixedUpdate () {
			if (Input.GetKeyDown (KeyCode.Space)) {
				GameObject.Instantiate (this.logo, this.transform);
			}
		}
	}
}