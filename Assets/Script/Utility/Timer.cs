using System;

namespace Game.Utility {
	using Component.Network;

	public class Timer {
		public delegate void Delegate();

		public bool IsRunning {
			get {
				return this.isRunning;
			}
		}

		public float Process {
			get {
				return this.value / this.max;
			}
		}

		public float value;
		public float max;
		private bool isRunning;
		private Delegate OnComplete;

		public Timer() {
			this.value = 0;
			this.max = 0;
			this.isRunning = false;
		}

		public void Update(float dt=Client.STDDT) {
			if (!this.isRunning) {
				return;
			}

			this.value += dt;

			if (this.value >= this.max) {
				this.value = this.max;
				this.Exit();
				this.OnComplete();
			}
		}

		public void Enter(float time=0, Delegate OnComplete=null) {
			this.value = 0;
			this.isRunning = true;
			this.max = time == 0 ? this.max : time.ToFixed();
			this.OnComplete = OnComplete == null ? this.OnComplete : OnComplete;
		}

		public void Exit() {
			this.isRunning = false;
		}
	}
}
