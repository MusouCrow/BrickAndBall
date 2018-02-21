using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Field {
	using Utility;

	public class Wall : MonoBehaviour {
		[SerializeField]
		private Transform barTransform;
		[SerializeField]
		private Vector4 shakingValue;

		private Tweener tweener;
		[System.NonSerialized]
		public Vector3 scale;
		private Collider barCollider;

		protected void Awake() {
			var collider = this.GetComponent<Collider>();
			collider.CollisionEnterEvent += this.OnCollide;

			this.scale = this.barTransform.localScale;
			this.barCollider = this.barTransform.GetComponent<Collider>();
			this.barCollider.CollisionEnterEvent += this.OnBarCollide;
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

		private bool CheckedFunc(int type, float pos, float point, float velocity) {
			return (point > pos && velocity > 0) || (point < pos && velocity < 0);
		}

		private void OnCollide(Collider collider, Vector3 point) {
			this.OnBarCollide(collider, point);

			if (this.tweener == null || !this.tweener.IsPlaying()) {
				this.tweener = this.transform.DOPunchPosition(this.shakingValue, this.shakingValue.w)
					.SetEase(Ease.InOutElastic);
			}
		}

		private void OnBarCollide(Collider collider, Vector3 point) {
			var ball = collider.GetComponent<Ball>();

			if (ball != null) {
				ball.Rebound(point, this.CheckedFunc);
				ball.Move(ball.Rate * ball.Velocity.x.ToDirection(), 0, ball.Rate * 2 * ball.Velocity.z.ToDirection());
			}
		}
	}
}