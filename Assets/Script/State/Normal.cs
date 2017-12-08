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
		private bool coolDown;
		private Timer timer;
		private int process;
		private Color originColor;
		private Color targetColor;

		public Normal (GameObject gameObject, Utility.SimpleJSON.JSONNode param) : base (gameObject, param) {
			this.controller = gameObject.GetComponent<Controller> ();
			this.coolDownTime = param ["coolDownTime"].AsFloat;
			this.coolDown = false;
			this.timer = new Timer ();
		}

		public override void Update() {
			if (!this.coolDown) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);
			this.controller.ColorLert (this.originColor, this.targetColor, this.timer.GetProcess ());

			if (!this.timer.isRunning) {
				this.process += 1;

				if (this.process <= 2) {
					this.timer.Enter (0.3f);

					if (this.process == 1) {
						this.originColor = this.controller.originColor;
						this.targetColor = Color.white;
					} else {
						this.originColor = Color.white;
						this.targetColor = this.controller.originColor;
					}
				} else {
					this.coolDown = false;
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

			if (Time.fixedTime - this.clickTime <= 0.2f) {
				this.statemgr.Play ("Elast");
			}

			this.clickTime = Time.fixedTime;
		}
	}
}

