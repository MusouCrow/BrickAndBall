using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Protector : MonoBehaviour {
		public float time;
		public float power;
		[SerializeField]
		private float end;
		[SerializeField]
		private AudioClip clip;
		public Vector4 shakingValue;

		private float begin;

		void Awake () {
			this.begin = this.transform.localPosition.x;
		}

		void OnEnable() {
			Sequence s = DOTween.Sequence ();
			Tweener t1 = this.transform.DOLocalMoveX (this.end, this.time)
				.SetEase (Ease.InOutBack);
			Tweener t2 = this.transform.DOLocalMoveX (this.begin, this.time)
				.SetEase (Ease.InOutQuad);

			s.Append (t1);
			s.Append (t2);
			s.AppendCallback (() => this.SetActive (false));

			Sound.Play (this.clip);
			ViceCamera.Shake (this.shakingValue);
		}

		public void SetActive(bool value) {
			this.gameObject.SetActive (value);
		}

		void OnCollisionEnter(Collision collision) {
			Ball ball = collision.gameObject.GetComponent<Ball> ();

			if (ball != null) {
				float power = collision.rigidbody.velocity.x > 0 ? this.power : -this.power;
				ball.Move (power, 0, 0);
			}
		}
	}
}