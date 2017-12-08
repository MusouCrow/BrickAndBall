using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Shooter : MonoBehaviour {
		public GameObject ball;

		void Update() {
			if (Input.GetKeyDown (KeyCode.Space)) {
				this.Shoot ();
			}
		}

		void Shoot() {
			GameObject obj = GameObject.Instantiate (this.ball, this.transform.localPosition, Quaternion.identity, this.transform.parent) as GameObject;
			obj.GetComponent<Rigidbody> ().AddForce (this.transform.localScale, ForceMode.VelocityChange);
		}
	}
}