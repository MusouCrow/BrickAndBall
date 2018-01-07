using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Shooter : MonoBehaviour {
		[SerializeField]
		private GameObject ball;
		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private Vector4 shakingValue;

		public GameObject Shoot() {
			GameObject obj = GameObject.Instantiate (this.ball, this.transform.localPosition, Quaternion.identity, this.transform.parent) as GameObject;
			obj.GetComponent<Rigidbody> ().AddForce (this.transform.localScale, ForceMode.VelocityChange);
			Sound.Play (this.clip);
			ViceCamera.Shake (this.shakingValue);

			return obj;
		}
	}
}