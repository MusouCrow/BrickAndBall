using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Random = UnityEngine.Random;

namespace Game.Component {
	using Utility;
	using Network;

	public class Brick : Controller {
		public class Dragging {
			public delegate void Delegate(Vector3 oldPos, Vector3 newPos);
			
			public event Delegate OnDragEvent;
			private Vector3 position;
			private bool hasDraged;
			private Collider collider;

			public Dragging(Collider collider) {
				this.collider = collider;
			}

			public void Drag(Vector3 position, bool isDown) {
				if (!isDown) {
					this.hasDraged = false;
				}
				else if (!this.hasDraged && this.collider.Pointcast(position)) {
                    this.hasDraged = true;
                }
				else if (this.hasDraged && this.OnDragEvent != null){
                    this.OnDragEvent(this.position, position);
				}

				if (this.hasDraged) {
					this.position = position;
				}
			}
		}

		public delegate void Delegate(Vector3 pos);
		private const float RANGE_Z = 2.3f;
		private static Vector2 POWER_X = new Vector2(6, 9);
		private static Vector2 POWER_Z = new Vector2(-5, 5);
		private static Vector2 AI_MOTION_TIME = new Vector2(0.4f, 0.7f);
		private static Vector4 SHAKING_VALUE = new Vector4(0.1f, 0, 0, 0.2f);

		private static void HandleValueWithRange(ref float value) {
			if (value > RANGE_Z) {
				value = RANGE_Z;
			} else if (value < -RANGE_Z) {
				value = -RANGE_Z;
			}
		}

		public event Delegate AdjustPositionEvent;
		public Dragging dragging;
		private new Collider collider;
		private Vector3 draggingPos;
		private Vector3 shakingPos;
		private Vector3 shakingValue;
		private Vector3 position;
		private Vector3 scale;
		private Tweener tweener;

		protected new void Awake() {
			base.Awake();

			this.collider = this.GetComponent<Collider>();
			this.collider.CollisionEnterEvent += this.OnCollide;

			this.dragging = new Dragging(this.collider);
			this.position = this.transform.localPosition;
			this.scale = this.transform.localScale;
			this.shakingValue = SHAKING_VALUE * this.direction;

			this.ResetEvent += this.ResetPostion;
			this.AITickEvent += this.FollowBall;
			this.dragging.OnDragEvent += this.OnDrag;
		}

		protected override void LockUpdate() {
			base.LockUpdate();

			if (Judge.GameType == GameType.PVE && this.isPlayer) {
				this.dragging.Drag(Client.MousePosition, Input.GetMouseButton(0));
			}
		}

		public Tweener MovePosition(int type, float target, float time) {
			return Math.MoveFixedFloat((float v) => {this.position[type] = v; this.AdjustPosition();}, this.position[type], target, time);
		}

		public Tweener MoveScale(int type, float target, float time) {
			return Math.MoveFixedFloat((float v) => {this.scale[type] = v; this.collider.Scale = this.scale;}, this.scale[type], target, time);
		}

		private void OnDrag(Vector3 oldPos, Vector3 newPos) {
			this.position.z += newPos.z - oldPos.z;
			Brick.HandleValueWithRange(ref this.position.z);
			this.AdjustPosition();
		}

		private bool CheckedFunc(int type, float pos, float point, float velocity) {
			return true;
		}

		private void OnCollide(Collider collider, Vector3 point) {
			var ball = collider.GetComponent<Ball>();
			
			if (ball != null) {
				ball.Rebound(point, this.CheckedFunc);
				
				float valueX = Math.Lerp(POWER_X.x, POWER_X.y, Math.Random());
				float valueZ = Math.Lerp(POWER_Z.x, POWER_Z.y, Math.Random());

				var velocity = ball.Velocity;
				velocity.x = Mathf.Abs(velocity.x) < 15 ? 0 : velocity.x - 15 * direction;
				velocity.x += valueX * this.direction * ball.Rate;
				velocity.z = valueZ * ball.Rate;
				ball.Velocity = velocity;
			}
			
			if (this.tweener == null || !this.tweener.IsPlaying()) {
				this.tweener = DOTween.Punch(this.GetShakingPos, this.SetShakingPos, this.shakingValue, SHAKING_VALUE.w);
			}
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
			this.MovePosition(2, ballPosition.z, Math.Lerp(AI_MOTION_TIME.x, AI_MOTION_TIME.y, Math.Random()));
		}
	}
}