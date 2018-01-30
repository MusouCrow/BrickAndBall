using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Wall : MonoBehaviour {
		private static float POWER = 3;

		[SerializeField]
		private Transform barTransform;
		[SerializeField]
		private Vector4 shakingValue;

		private Tweener tweener;
		private Vector3 scale;
		private Collider barCollider;

		protected void Awake() {
			var collider = this.GetComponent<Collider>();
			collider.CollisionEnterEvent += this.OnCollide;

			this.scale = this.barTransform.localScale;
			this.barCollider = this.barTransform.GetComponent<Collider>();
		}

		public void SetLength(float value) {
			Math.MoveFixedFloat((float v) => {
					this.scale.x = v;
					this.barCollider.Scale = this.scale;
				}, this.scale.x, value, 1)
				.SetEase(Ease.OutBack);
		}

		public void Reset() {
			this.SetLength(0.01f);
		}

		private void OnCollide(Collider collider) {
			var ball = collider.GetComponent<Ball>();

			if (ball != null) {
				var power = ball.Velocity.x > 0 ? ball.Rate : -ball.Rate;
				ball.Move(power, 0, 0);
			}

			if (this.tweener == null || !this.tweener.IsPlaying()) {
				this.tweener = this.transform.DOPunchPosition(this.shakingValue, this.shakingValue.w)
					.SetEase(Ease.InOutElastic);
			}
		}
	}
}