using System;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace Game.Utility {
	public class Shaking {
		private Transform transform;
		private Timer timer;
		private Vector3 position;
		private Vector3 power;
		private Random random;

		public Shaking (Transform transform) {
			this.transform = transform;
			this.timer = new Timer ();
			this.random = new Random ();
		}

		public void Update(float dt) {
			if (!this.timer.isRunning) {
				return;
			}

			this.timer.Update (dt);

			if (!this.timer.isRunning) {
				this.Exit ();
			} else {
				this.Shake (Vector3.Lerp (-this.power, this.power, (float)this.random.NextDouble ()));
			}
		}

		public void Enter(Vector3 power, float time) {
			if (this.timer.isRunning) {
				this.Exit ();
			}

			this.position = this.transform.localPosition;
			this.power = power;
			this.timer.Enter (time);
		}

		public void Enter(Vector4 value) {
			this.Enter (value, value.w);
		}

		public void Exit() {
			this.timer.isRunning = false;
			this.Shake (Vector3.zero);
		}

		private void Shake(Vector3 range) {
			this.transform.localPosition = this.position + range;
		}
	}
}

