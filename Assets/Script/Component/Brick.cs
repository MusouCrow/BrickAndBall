using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = System.Random;

namespace Game.Component {
	using Utility;

	public class Brick : Controller {
		private const float RANGE_Z = 2.5f;
		private static Vector2 POWER_X = new Vector2 (7, 12);
		private static Vector2 POWER_Z = new Vector2 (5, 15);
		private static Vector2 AI_INTERVAL = new Vector2 (0.3f, 0.5f);
		private static Vector2 AI_MOTION_TIME = new Vector2 (0.2f, 0.6f);
		private static Vector4 SHAKING_VALUE = new Vector4 (0.1f, 0, 0, 0.2f);

		private static void HandleValueWithRange (ref float value) {
			if (value > RANGE_Z) {
				value = RANGE_Z;
			} else if (value < -RANGE_Z) {
				value = -RANGE_Z;
			}
		}

		private Random random;
		private Vector3 draggingPos;
		private Vector3 shakingPos;
		private Vector3 position;
		private Timer timer;

		protected new void Awake () {
			base.Awake ();

			this.random = new Random ();
			this.position = this.transform.localPosition;
			this.ResetEvent += this.ResetPostion;
			this.timer = new Timer (Mathf.Lerp (AI_INTERVAL.x, AI_INTERVAL.y, (float)this.random.NextDouble ()));
		}

		protected void FixedUpdate () {
			if (!this.isRunning || this.identity != Identity.AI) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);

			if (!this.timer.IsRunning ()) {
				Vector3 pos = Judge.GetBallPosition ();
				Brick.HandleValueWithRange (ref pos.z);
				this.MovePosition (2, pos.z, Mathf.Lerp (AI_MOTION_TIME.x, AI_MOTION_TIME.y, (float)this.random.NextDouble ()));
				this.timer.Enter (Mathf.Lerp (AI_INTERVAL.x, AI_INTERVAL.y, (float)this.random.NextDouble ()));
			}
		}

		protected void OnMouseDown() {
			if (!this.CanConroll ()) {
				return;
			}

			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
		}

		protected void OnMouseDrag () {
			if (!this.CanConroll ()) {
				return;
			}

			Vector3 oldPos = this.draggingPos;
			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = this.draggingPos;

			this.position.z += newPos.z - oldPos.z;
			Brick.HandleValueWithRange (ref this.position.z);
			this.AdjustPosition ();
		}

		protected void OnCollisionEnter (Collision collision) {
			Ball ball = collision.gameObject.GetComponent<Ball> ();

			if (ball != null) {
				float valueX = Mathf.Lerp (POWER_X.x, POWER_X.y, (float)this.random.NextDouble ());
				float valueZ = Mathf.Lerp (POWER_Z.x, POWER_Z.y, (float)this.random.NextDouble ());
				
				valueX = collision.rigidbody.velocity.x > 0 ? valueX : -valueX;
				valueZ = this.random.NextDouble () < 0.5 ? valueZ : -valueZ;
				ball.Move(valueX, 0, valueZ);
			}

			DOTween.Punch (this.GetShakingPos, this.SetShakingPos, SHAKING_VALUE, SHAKING_VALUE.w)
				.OnUpdate (this.AdjustPosition);
		}

		private void AdjustPosition () {
			this.transform.localPosition = this.position + this.shakingPos;
		}

		public Tweener MovePosition (int type, float target, float time) {
			return DOTween.To (v => this.position [type] = v, this.position [type], target, time)
				.OnUpdate (this.AdjustPosition);
		}

		private void ResetPostion () {
			this.MovePosition (2, 0, 1);
		}

		private Vector3 GetShakingPos () {
			return this.shakingPos;
		}

		private void SetShakingPos (Vector3 value) {
			this.shakingPos = value;
		}
	}
}