using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
		private Shaking shaking;
		private Timer timer;

		[NonSerialized]
		public Vector3 position;

		protected new void Awake () {
			base.Awake ();

			this.random = new Random ();
			this.shaking = new Shaking ();
			this.timer = new Timer ();
			this.position = this.transform.localPosition;
		}

		protected void FixedUpdate() {
			if (this.timer.IsRunning ()) {
				this.timer.Update (Time.fixedDeltaTime);
				this.position.z = Mathf.Lerp (this.position.z, 0, this.timer.GetProcess ());
				this.AdjustPosition ();
			}

			if (this.shaking.IsRunning ()) {
				this.shaking.Update (Time.fixedDeltaTime);
				this.AdjustPosition ();
			}
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

			this.shaking.Enter (this.shakingValue);
		}

		public void AdjustPosition () {
			this.transform.localPosition = this.position + this.shaking.GetPosition ();
		}

		public void Reset (float time) {
			this.timer.Enter (time);
		}
	}
}