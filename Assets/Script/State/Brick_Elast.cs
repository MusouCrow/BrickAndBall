using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State {
	using Component;
	using Utility;
	using Utility.SimpleJSON;

	public class Brick_Elast : State {
		public struct Scale
		{
			public float start;
			public float end;
			public float time;
		}

		private Transform transform;
		private Controller controller;
		private Scale scale;
		private AudioClip clip;
		private float power;
		private string nextState;
		private Timer timer;
		private int process;

		public Brick_Elast (GameObject gameObject, JSONNode param) : base (gameObject, param) {
			this.transform = gameObject.GetComponent<Transform> ();
			this.controller = gameObject.GetComponent<Controller> ();

			this.scale = new Scale {
				start = param ["scale"] ["start"].AsFloat,
				end = param ["scale"] ["end"].AsFloat,
				time = param ["scale"] ["time"].AsFloat
			};

			this.clip = Resources.Load (param ["clip"].Value) as AudioClip;
			this.power = param ["power"].AsFloat;
			this.nextState = param ["nextState"].Value;
			this.timer = new Timer ();
			this.process = 0;
		}

		public override void Update () {
			this.timer.Update (Time.fixedDeltaTime);

			if (this.process == 1) {
				this.controller.ColorLert (this.controller.originColor, this.controller.targetColor, this.timer.GetProcess ());
			}

			float start = 0;
			float end = 0;

			if (this.process == 0) {
				start = this.scale.start;
				end = this.scale.end;
			} else {
				start = this.scale.end;
				end = this.scale.start;
			}

			Vector3 scale = this.transform.localScale;
			scale.x = Mathf.Lerp (start, end, this.timer.value / this.scale.time);
			this.transform.localScale = scale;

			if (!this.timer.isRunning) {
				if (this.process == 0) {
					this.process = 1;
					this.timer.Enter (this.scale.time);
				} else {
					this.statemgr.Play (this.nextState);
				}
			}
		}

		public override void Enter() {
			this.timer.Enter (this.scale.time);
			AudioSource.PlayClipAtPoint (this.clip, Vector3.zero);
			this.process = 0;
		}

		public override void OnCollisionEnter(Collision collision) {
			if (collision.rigidbody != null) {
				Vector3 velocity = collision.rigidbody.velocity;

				if (velocity.x > 0) {
					velocity.x += this.power;
				} else {
					velocity.x -= this.power;
				}

				collision.rigidbody.velocity = velocity;
			}
		}
	}
}

