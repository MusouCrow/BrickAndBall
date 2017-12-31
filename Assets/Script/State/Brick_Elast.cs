using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State {
	using Component;
	using Utility;
	using StateData = Data.Brick_Elast;

	public class Brick_Elast : State {
		private StateData data;
		private Transform transform;
		private Brick brick;
		private Timer timer;
		private int process;

		public Brick_Elast (GameObject gameObject, StateData data) : base (gameObject, data) {
			this.data = data;
			this.transform = gameObject.GetComponent<Transform> ();
			this.brick = gameObject.GetComponent<Brick> ();
			this.timer = new Timer ();
			this.process = 0;
		}

		public override void Update () {
			this.timer.Update (Time.fixedDeltaTime);

			int start = 0;
			int end = 1;
			float timeProcess = this.timer.GetProcess ();

			if (this.process == 1) {
				start = 1;
				end = 0;
				this.brick.ColorLert (this.brick.originColor, this.brick.targetColor, timeProcess);
			}

			Vector3 scale = this.transform.localScale;
			scale.x = Mathf.Lerp (this.data.scaling [start], this.data.scaling [end], timeProcess);
			this.transform.localScale = scale;

			this.brick.position.x = Mathf.Lerp (this.data.positioning [start], this.data.positioning [end], timeProcess);
			this.brick.position.x *= -this.brick.direction;
			this.brick.AdjustPosition ();

			if (!this.timer.IsRunning ()) {
				if (this.process == 0) {
					this.process = 1;
					this.timer.Enter (this.data.time);
				} else {
					this.statemgr.Play (this.data.nextState);
				}
			}
		}

		public override void Enter() {
			this.timer.Enter (this.data.time);
			Sound.Play (this.data.clip);
			this.process = 0;
		}

		public override void OnCollisionEnter(Collision collision) {
			Ball ball = collision.gameObject.GetComponent<Ball> ();

			if (ball != null) {
				float power = collision.rigidbody.velocity.x > 0 ? this.data.power : -this.data.power;
				ball.Move (power, 0, 0);
			}
		}
	}
}

