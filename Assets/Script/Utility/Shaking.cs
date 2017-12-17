using System;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace Game.Utility {
	public class Shaking { 
		private Timer timer;
		private Vector3 position;
		private Vector3 power;
		private Random random;

		public Shaking () {
			this.timer = new Timer ();
			this.random = new Random ();
		}

		public void Update(float dt) {
			if (!this.timer.IsRunning ()) {
				return;
			}

			this.timer.Update (dt);

			if (!this.timer.IsRunning ()) {
				this.Exit ();
			} else {
				this.position = Vector3.Lerp (-this.power, this.power, (float)this.random.NextDouble ());
			}
		}

		public void Enter(Vector3 power, float time) {
			if (this.timer.IsRunning ()) {
				this.Exit ();
			}

			this.position = Vector3.zero;
			this.power = power;
			this.timer.Enter (time);
		}

		public void Enter(Vector4 value) {
			this.Enter (value, value.w);
		}

		public void Exit() {
			this.timer.Exit ();
			this.position = Vector3.zero;
		}

		public Vector3 GetPosition() {
			return this.position;
		}

		public bool IsRunning() {
			return this.timer.IsRunning ();
		}
	}
}

