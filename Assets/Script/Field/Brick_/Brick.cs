using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Random = UnityEngine.Random;
using UInput = UnityEngine.Input;

namespace Game.Field.Brick_ {
	using Utility;
	using Network;

	public class Brick : LockBehaviour {
		private const float RANGE_Z = 2.3f;
		private static Vector2 POWER_X = new Vector2(6, 9);
		private static Vector2 POWER_Z = new Vector2(-5, 5);
		private static Vector2 AI_MOTION_TIME = new Vector2(0.3f, 0.5f);
		private static Vector2 AI_INTERAL_RANGE = new Vector2(0.2f, 0.5f);
		private const float NET_MOTION_TIME = 0.1f;
		private const float REBOUND_POWER = 12;

		private static void HandleValueWithRange(ref float value) {
			if (value > RANGE_Z) {
				value = RANGE_Z;
			} else if (value < -RANGE_Z) {
				value = -RANGE_Z;
			}
		}

		public Color targetColor;
		public int direction;

		[NonSerialized]
		public Color originColor;
		[NonSerialized]
		public bool isRunning;
		[NonSerialized]
		public bool isPlayer;
		[NonSerialized]
		public Statemgr statemgr;
		public event Action<Vector3> AdjustPositionEvent;
		public event Action<Vector3> AITickEvent;
		public event Action ResetEvent;
		public Dragging dragging;
		private new MeshRenderer renderer;
		private new Collider collider;
		private Timer timer;
		private Shaking shaking;
		private Vector3 position;
		private Vector3 scale;

		public bool CanConroll {
			get {
				return this.isRunning && this.isPlayer;
			}
		}

		public float AIInterval {
			get {
				return Math.Lerp(AI_INTERAL_RANGE.x, AI_INTERAL_RANGE.y, Math.Random());
			}
		}

		protected new void Awake() {
			base.Awake();

			this.statemgr = this.GetComponent<Statemgr>();
			this.renderer = this.GetComponent<MeshRenderer>();
			this.originColor = this.renderer.material.color;
			this.shaking = new Shaking(this.direction, this.AdjustPosition);

			this.timer = new Timer ();
			this.timer.Enter(1, this.TickAI);

			this.collider = this.GetComponent<Collider>();
			this.collider.CollisionEnterEvent += this.OnCollide;

			this.dragging = new Dragging(this.collider);
			this.dragging.OnDragEvent += this.OnDrag;

			this.position = this.transform.localPosition;
			this.scale = this.transform.localScale;

			this.ResetEvent += this.ResetPostion;
			this.AITickEvent += this.FollowBall;
		}

		protected override void LockUpdate() {
			if (!this.isRunning) {
				return;
			}

			if (Judge.GameType == GameType.PVE && !this.isPlayer) {
				this.timer.Update();
			}

			if (this.isPlayer) {
				this.dragging.Drag(ViceCamera.MousePosition, UInput.GetMouseButton(0));
			}
		}

		public void Reset() {
			if (this.ResetEvent != null) {
				this.ResetEvent();
			}
		}

		public void Input(InputData inputData) {
			this.MovePosition(2, inputData.movingValue, NET_MOTION_TIME).SetEase(Ease.OutExpo);

			if (inputData.willElaste) {
				this.statemgr.Play("Elast");
			}
		}

		public Tweener MoveColor(Color value, float t) {
			return this.renderer.material.DOColor(value, t.ToFixed())
				.SetEase(Ease.Linear);
		}

		public Tweener MovePosition(int type, float target, float time) {
			return Math.MoveFixedFloat((float v) => {this.position[type] = v; this.AdjustPosition();}, this.position[type], target, time);
		}

		public Tweener MoveScale(int type, float target, float time) {
			return Math.MoveFixedFloat((float v) => {this.scale[type] = v; this.collider.Scale = this.scale;}, this.scale[type], target, time);
		}

		private void TickAI() {
			if (this.AITickEvent != null) {
				this.AITickEvent(Ball.Position);
			}

			this.timer.Enter(this.AIInterval, this.TickAI);
		}

		private void OnDrag(Vector3 oldPos, Vector3 newPos) {
			if (Judge.GameType == GameType.PVE) {
				this.position.z += newPos.z - oldPos.z;
				Brick.HandleValueWithRange(ref this.position.z);
				this.AdjustPosition();
			}
			else {
				var value = Networkmgr.MovingValue;
				value += newPos.z - oldPos.z;
				Brick.HandleValueWithRange(ref value);
				Networkmgr.MovingValue = value;
			}
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

				var velocity = Ball.Velocity;
				velocity.x = Mathf.Abs(velocity.x) < REBOUND_POWER ? 0 : velocity.x - REBOUND_POWER * direction;
				velocity.x += valueX * this.direction * Judge.Rate;
				velocity.z = valueZ * Judge.Rate;
				Ball.Velocity = velocity;
				this.shaking.Collide();

				if (this.isPlayer) {
					Vibrate.Do((int)(velocity.magnitude * 10));
				}
			}
		}

		private void ResetPostion() {
			this.MovePosition(2, 0, 1);
		}

		private void AdjustPosition() {
			this.collider.Position = this.position + this.shaking.position;

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