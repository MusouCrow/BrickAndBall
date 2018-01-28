using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Shooter : MonoBehaviour {
		public delegate void Delegate(GameObject obj);
		public static event Delegate OnShootEvent;

		[SerializeField]
		private GameObject ball;
		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private Vector4 shakingValue;

		private GameObject NewBall() {
			var obj = GameObject.Instantiate(this.ball, this.transform.localPosition, Quaternion.identity, this.transform.parent);
			obj.GetComponent<Collider>().AddForce(this.transform.localScale);
		
			return obj;
		}

		public void Shoot() {
			if (Shooter.OnShootEvent != null) {
				Shooter.OnShootEvent(this.NewBall());
			}

			Sound.Play(this.clip);
			ViceCamera.Shake(this.shakingValue);
		}
	}
}