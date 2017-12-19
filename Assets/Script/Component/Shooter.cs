using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Shooter : MonoBehaviour {
		[SerializeField]
		private GameObject ball;
		[SerializeField]
		private AudioClip clip;

		public Vector4 shakingValue;

		public GameObject Shoot() {
			GameObject obj = GameObject.Instantiate (this.ball, this.transform.localPosition, Quaternion.identity, this.transform.parent) as GameObject;
			obj.GetComponent<Rigidbody> ().AddForce (this.transform.localScale, ForceMode.VelocityChange);
			AudioSource.PlayClipAtPoint (this.clip, Vector3.zero);
			ViceCamera.Shake (this.shakingValue);

			return obj;
		}
	}
}