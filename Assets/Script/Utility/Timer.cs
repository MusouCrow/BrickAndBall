using System;

namespace Game.Utility {
	public class Timer {
		public float value;
		public float max;
		public bool isRunning;

		public Timer() {
			this.value = 0;
			this.max = 0;
			this.isRunning = false;
		}

		public void Update (float dt) {
			if (!this.isRunning) {
				return;
			}

			this.value += dt;

			if (this.value >= this.max) {
				this.value = this.max;
				this.isRunning = false;
			}
		}

		public void Enter(float time, bool inherited=false) {
			this.isRunning = true;

			if (!inherited) {
				this.value = 0;
			}

			this.max = time;

			if (this.value >= this.max) {
				this.value = this.value - this.max;
				this.isRunning = false;
			}
		}

		public float GetProcess() {
			return this.value / this.max;
		}
	}
}

