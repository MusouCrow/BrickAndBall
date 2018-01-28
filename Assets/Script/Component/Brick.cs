using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Random = UnityEngine.Random;

namespace Game.Component {
	using Utility;

	public class Brick : Controller {
		public delegate void Delegate(Vector3 pos);
		private const float RANGE_Z = 2.5f;
		private static Vector2 POWER_X = new Vector2(7, 12);
		private static Vector2 POWER_Z = new Vector2(5, 15);
		private static Vector2 AI_MOTION_TIME = new Vector2(0.2f, 0.6f);
		private static Vector4 SHAKING_VALUE = new Vector4(0.1f, 0, 0, 0.2f);

		private static void HandleValueWithRange(ref float value) {
			if (value > RANGE_Z) {
				value = RANGE_Z;
			} else if (value < -RANGE_Z) {
				value = -RANGE_Z;
			}
		}

		public event Delegate AdjustPositionEvent;
		private new Collider collider;
		private Vector3 draggingPos;
		private Vector3 shakingPos;
		private Vector3 shakingValue;
		private Vector3 position;
		private Vector3 scale;

		protected new void Awake() {
			base.Awake();

			this.collider = this.GetComponent<Collider>();
			//this.collider.CollisionEnterEvent += this.OnCollide;

			this.position = this.transform.localPosition;
			this.scale = this.transform.localScale;
			this.shakingValue = SHAKING_VALUE * this.direction;

			this.ResetEvent += this.ResetPostion;
			this.AITickEvent += this.FollowBall;
		}

		protected void OnMouseDown() {
			if (!this.CanConroll()) {
				return;
			}

			this.draggingPos = ViceCamera.ScreenToWorldPoint(Input.mousePosition);
		}

		protected void OnMouseDrag() {
			if (!this.CanConroll()) {
				return;
			}

			Vector3 oldPos = this.draggingPos;
			this.draggingPos = ViceCamera.ScreenToWorldPoint(Input.mousePosition);
			Vector3 newPos = this.draggingPos;

			this.position.z += (newPos.z - oldPos.z).ToFixed();
			Brick.HandleValueWithRange(ref this.position.z);
			this.AdjustPosition();
		}

		public Tweener MovePosition(int type, float target, float time) {
			return DOTween.To((float v) => {this.position[type] = v.ToFixed(); this.AdjustPosition();}, this.position[type], target, time);
		}

		public Tweener MoveScale(int type, float target, float time) {
			return DOTween.To((float v) => {this.scale[type] = v.ToFixed(); this.collider.Scale = this.scale;}, this.scale[type], target, time);
		}

		private void OnCollide(Collider collider) {
			var ball = collider.GetComponent<Ball>();

			if (ball != null) {
				float valueX = Mathf.Lerp (POWER_X.x, POWER_X.y, Random.value);
				float valueZ = Mathf.Lerp (POWER_Z.x, POWER_Z.y, Random.value);

				valueX = ball.velocity.x > 0 ? valueX : -valueX;
				valueZ = Random.value < 0.5f ? valueZ : -valueZ;
				ball.Move(valueX, 0, valueZ);
			}
			
			DOTween.Punch(this.GetShakingPos, this.SetShakingPos, this.shakingValue, SHAKING_VALUE.w);
		}

		private Vector3 GetShakingPos() {
			return this.shakingPos;
		}

		private void SetShakingPos(Vector3 value) {
			this.shakingPos = value.ToFixed();
			this.AdjustPosition();
		}

		private void ResetPostion() {
			this.MovePosition(2, 0, 1);
		}

		private void AdjustPosition() {
			this.collider.Position = this.position + this.shakingPos;

			if (this.AdjustPositionEvent != null) {
				this.AdjustPositionEvent(this.transform.localPosition);
			}
		}

		private void FollowBall(Vector3 ballPosition) {
			Brick.HandleValueWithRange(ref ballPosition.z);
			int direction = 1;

			if ((this.direction == 1 && ballPosition.x < this.transform.localPosition.x)
				|| (this.direction == -1 && ballPosition.x > this.transform.localPosition.x)) {
				direction = -1;
			}

			this.MovePosition (2, ballPosition.z * direction, Mathf.Lerp (AI_MOTION_TIME.x, AI_MOTION_TIME.y, Random.value));
		}
	}
}