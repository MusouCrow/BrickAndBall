using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component {
	public class Shooter : MonoBehaviour {
		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private Vector4 shakingValue;

		public void Shoot(Ball ball) {
			ball.transform.localPosition = this.transform.localPosition;
			ball.gameObject.SetActive(true);
			ball.GetComponent<Collider>().AddForce(this.transform.localScale);

			Sound.Play(this.clip);
			ViceCamera.Shake(this.shakingValue);
		}
	}
}