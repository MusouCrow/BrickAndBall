using System;

namespace Game.Utility {
	public class Timer {
		public float value;
		public float max;
		private bool isRunning;

		public Timer(float time=0) {
			this.value = 0;
			this.max = 0;
			this.isRunning = false;

			if (time > 0) {
				this.Enter (time);
			}
		}

		public void Update (float dt) {
			if (!this.isRunning) {
				return;
			}

			this.value += dt;

			if (this.value >= this.max) {
				this.value = this.max;
				this.Exit ();
			}
		}

		public void Enter(float time=0, bool inherited=false) {
			this.isRunning = true;

			if (!inherited) {
				this.value = 0;
			}

			if (time != 0) {
				this.max = time;
			}

			if (this.value >= this.max) {
				this.value = this.value - this.max;
				this.isRunning = false;
			}
		}

		public void Exit() {
			this.isRunning = false;
		}

		public bool IsRunning() {
			return this.isRunning;
		}

		public float GetProcess() {
			return this.value / this.max;
		}
	}
}
