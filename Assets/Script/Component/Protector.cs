using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Protector : MonoBehaviour {
		private static float POWER = 15;

		public float time;
		[SerializeField]
		private float end;
		[SerializeField]
		private AudioClip clip;
		[SerializeField]
		private Vector4 shakingValue;

		private float begin;
		private float process;
		private new Collider collider;

		protected void Awake () {
			this.collider = this.GetComponent<Collider>();
			this.collider.CollisionEnterEvent += this.OnCollide;

			this.begin = this.transform.localPosition.x;
			this.process = this.begin;
		}

		protected void OnEnable() {
			var s = DOTween.Sequence();
			var t1 = this.MoveProcess(this.end, this.time)
				.SetEase(Ease.InOutBack);
			var t2 = this.MoveProcess(this.begin, this.time)
				.SetEase(Ease.InOutQuad);

			s.Append (t1);
			s.Append (t2);
			s.AppendCallback(() => this.SetActive(false));

			Sound.Play(this.clip);
			ViceCamera.Shake(this.shakingValue);
		}

		public void SetActive(bool value) {
			this.gameObject.SetActive(value);
		}

		private Tweener MoveProcess(float target, float time) {
			return DOTween.To((float v) => {
					this.process = v.ToFixed();
					var pos = this.collider.Position;
					pos.x = this.process;
					this.collider.Position = pos;
				}, this.process, target, time);
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