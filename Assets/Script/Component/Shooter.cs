using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Shooter : MonoBehaviour {
		public GameObject ball;
		
		public GameObject Shoot() {
			GameObject obj = GameObject.Instantiate (this.ball, this.transform.localPosition, Quaternion.identity, this.transform.parent) as GameObject;
			obj.GetComponent<Rigidbody> ().AddForce (this.transform.localScale, ForceMode.VelocityChange);

			return obj;
		}
	}
}