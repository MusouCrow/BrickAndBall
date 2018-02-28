using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Field {
	public class Shooter : MonoBehaviour {
		private const int VIBRATE_TIME = 200;

		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private Vector4 shakingValue;
		[SerializeField]
		private Ball ball;

		public void Shoot() {
			this.ball.transform.localPosition = this.transform.localPosition;
			this.ball.gameObject.SetActive(true);
			this.ball.GetComponent<Collider>().AddForce(this.transform.localScale);

			Sound.Play(this.clip);
			ViceCamera.Shake(this.shakingValue);
			Vibrate.Do(VIBRATE_TIME);
		}
	}
}