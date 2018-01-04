using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = System.Random;

namespace Game.Component {
	using Utility;

	public class Brick : Controller {
		private const float RANGE = 2.5f;

		public Vector2 powerX;
		public Vector2 powerZ;
		public Vector4 shakingValue;

		private Random random;
		private Vector3 draggingPos;
		private Vector3 shakingPos;
		private Vector3 position;

		protected new void Awake () {
			base.Awake ();

			this.random = new Random ();
			this.position = this.transform.localPosition;
		}

		protected void OnMouseDown() {
			if (!this.canControll) {
				return;
			}

			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
		}

		protected void OnMouseDrag () {
			if (!this.canControll) {
				return;
			}

			Vector3 oldPos = this.draggingPos;
			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = this.draggingPos;

			this.position.z += newPos.z - oldPos.z;

			if (this.position.z > RANGE) {
				this.position.z = RANGE;
			} else if (this.position.z < -RANGE) {
				this.position.z = -RANGE;
			}

			this.AdjustPosition ();
		}

		protected void OnCollisionEnter (Collision collision) {
			Ball ball = collision.gameObject.GetComponent<Ball> ();

			if (ball != null) {
				float valueX = Mathf.Lerp (this.powerX.x, this.powerX.y, (float)this.random.NextDouble ());
				float valueZ = Mathf.Lerp (this.powerZ.x, this.powerZ.y, (float)this.random.NextDouble ());
				
				valueX = collision.rigidbody.velocity.x > 0 ? valueX : -valueX;
				valueZ = this.random.NextDouble () < 0.5 ? valueZ : -valueZ;
				ball.Move(valueX, 0, valueZ);
			}

			DOTween.Punch (this.GetShakingPos, this.SetShakingPos, this.shakingValue, this.shakingValue.w)
				.OnUpdate (this.AdjustPosition);
		}

		private void AdjustPosition () {
			this.transform.localPosition = this.position + this.shakingPos;
		}

		public Tweener MovePosition (int type, float target, float time) {
			return DOTween.To (v => this.position [type] = v, this.position [type], target, time)
				.OnUpdate (this.AdjustPosition);
		}

		public void Reset (float time) {
			this.MovePosition (2, 0, time);
		}

		private Vector3 GetShakingPos () {
			return this.shakingPos;
		}

		private void SetShakingPos (Vector3 value) {
			this.shakingPos = value;
		}
	}
}