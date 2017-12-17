using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State {
	using Component;
	using Utility;
	using Utility.SimpleJSON;

	public class Brick_Elast : State {
		private Transform transform;
		private Brick brick;
		private float[] scaling;
		private float[] positioning;
		private float time;
		private AudioClip clip;
		private float power;
		private string nextState;
		private Timer timer;
		private int process;

		public Brick_Elast (GameObject gameObject, JSONNode param) : base (gameObject, param) {
			this.transform = gameObject.GetComponent<Transform> ();
			this.brick = gameObject.GetComponent<Brick> ();
			this.scaling = new float[]{ param ["scaling"] [0].AsFloat, param ["scaling"] [1].AsFloat };
			this.positioning = new float[]{ param ["positioning"] [0].AsFloat, param ["positioning"] [1].AsFloat };
			this.time = param ["time"].AsFloat;
			this.clip = Resources.Load (param ["clip"].Value) as AudioClip;
			this.power = param ["power"].AsFloat;
			this.nextState = param ["nextState"].Value;
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
			scale.x = Mathf.Lerp (this.scaling [start], this.scaling [end], timeProcess);
			this.transform.localScale = scale;

			this.brick.position.x = Mathf.Lerp (this.positioning [start], this.positioning [end], timeProcess);
			this.brick.position.x *= -this.brick.direction;
			this.brick.AdjustPosition ();

			if (!this.timer.IsRunning ()) {
				if (this.process == 0) {
					this.process = 1;
					this.timer.Enter (this.time);
				} else {
					this.statemgr.Play (this.nextState);
				}
			}
		}

		public override void Enter() {
			this.timer.Enter (this.time);
			AudioSource.PlayClipAtPoint (this.clip, Vector3.zero);
			this.process = 0;
		}

		public override void OnCollisionEnter(Collision collision) {
			Ball ball = collision.gameObject.GetComponent<Ball> ();

			if (ball != null) {
				float power = collision.rigidbody.velocity.x > 0 ? this.power : -this.power;
				ball.Move (power, 0, 0);
			}
		}
	}
}

