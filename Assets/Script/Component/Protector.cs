using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Protector : MonoBehaviour {
		private static float POWER = 20;

		public float time;
		[SerializeField]
		private float end;
		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private Vector4 shakingValue;

		private float begin;
		private Vector3 position;
		private new Collider collider;

		protected void Awake() {
			this.collider = this.GetComponent<Collider>();
			this.collider.CollisionEnterEvent += this.OnCollide;

			this.begin = this.transform.localPosition.x;
			this.position = this.transform.localPosition;
		}

		protected void OnEnable() {
			var s = DOTween.Sequence();
			var t1 = this.MoveProcess(this.end, this.time)
				.SetEase(Ease.InOutBack);
			var t2 = this.MoveProcess(this.begin, this.time)
				.SetEase(Ease.InOutQuad);

			s.Append(t1);
			s.Append(t2);
			s.AppendCallback(() => this.SetActive(false));

			Sound.Play(this.clip);
			ViceCamera.Shake(this.shakingValue);
		}

		public void SetActive(bool value) {
			this.gameObject.SetActive(value);
		}

		private Tweener MoveProcess(float target, float time) {
			return Math.MoveFixedFloat((float v) => {
					this.position.x = v;
					this.collider.Position = this.position;
				}, this.position.x, target, time);
		}

		private void OnCollide(Collider collider) {
			var ball = collider.GetComponent<Ball>();

			if (ball != null) {
				float power = ball.velocity.x > 0 ? POWER : -POWER;
				ball.Move(power, 0, 0);
			}
		}
	}
}