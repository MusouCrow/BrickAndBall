using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State {
	using Component;
	using Utility;

	public class Normal : State {
		private Controller controller;
		private float clickTime;
		private float coolDownTime;
		private float dragValue;
		private bool coolDown;
		private Timer timer;
		private int process;
		private Color originColor;
		private Color targetColor;
		private Dragging dragging;
		private AudioClip clip;

		public Normal (GameObject gameObject, Utility.SimpleJSON.JSONNode param) : base (gameObject, param) {
			this.controller = gameObject.GetComponent<Controller> ();
			this.coolDownTime = param ["coolDownTime"].AsFloat;
			this.clip = Resources.Load ("Sound/Ready") as AudioClip;
			this.dragValue = 0.5f;
			this.coolDown = false;
			this.timer = new Timer ();
			this.dragging = new Dragging ();
		}

		public override void Update() {
			if (!this.coolDown) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);
			this.controller.ColorLert (this.originColor, this.targetColor, this.timer.GetProcess ());

			if (!this.timer.IsRunning ()) {
				this.process += 1;

				if (this.process <= 2) {
					this.timer.Enter (0.3f);

					if (this.process == 1) {
						Sound.Play (this.clip);
						this.originColor = this.controller.originColor;
						this.targetColor = Color.white;
					} else {
						this.originColor = Color.white;
						this.targetColor = this.controller.originColor;
					}
				} else {
					this.coolDown = false;
					this.dragging.Update (Input.mousePosition);
				}
			}
		}

		public override void Enter() {
			this.timer.Enter (this.coolDownTime);
			this.process = 0;
			this.originColor = this.controller.targetColor;
			this.targetColor = this.controller.originColor;
		}

		public override void Exit() {
			this.coolDown = true;
		}

		public override void OnMouseDown () {
			if (this.coolDown) {
				return;
			}

			this.dragging.Update (Input.mousePosition);
		}

		public override void OnMouseDrag () {
			if (this.coolDown) {
				return;
			}

			Vector3 oldPos = this.dragging.GetPosition ();
			this.dragging.Update (Input.mousePosition);
			Vector3 newPos = this.dragging.GetPosition ();
			float delta = newPos.x - oldPos.x;

			if ((this.controller.direction > 0 && delta > this.dragValue) || (this.controller.direction < 0 && delta < -this.dragValue)) {
				this.statemgr.Play ("Elast");
			}
		}
	}
}

