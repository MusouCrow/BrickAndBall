using System;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace Game.Utility {
	public class Shaking {
		private Transform transform;
		private int type;
		private float value;
		private float range;
		private Timer timer;
		private Vector3 position;
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
				this.Shake (Mathf.Lerp (-this.range, this.range, (float)this.random.NextDouble ()));
			}
		}

		// type: 0 is x, 1 is y, 2 is z.
		public void Enter(int type, float range, float time) {
			if (this.timer.isRunning) {
				this.Exit ();
			}

			this.position = this.transform.localPosition;
			this.type = type;
			this.value = this.position [this.type];
			this.range = range;
			this.timer.Enter (time);
		}

		public void Exit() {
			this.timer.isRunning = false;
			this.Shake (0);
		}

		private void Shake(float range) {
			this.position = this.transform.localPosition;
			this.position [this.type] = this.value + range;
			this.transform.localPosition = this.position;
		}
	}
}

