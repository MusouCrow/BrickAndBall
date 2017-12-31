using System;
using UnityEngine;

namespace Game.State {
	using Component;
	using Utility;
	using StateData = Data.Mark_Elast;

	public class Mark_Elast : State {
		private StateData data;
		private Mark mark;
		private Timer timer;

		public Mark_Elast (GameObject gameObject, StateData data) : base (gameObject, data) {
			this.data = data;
			this.mark = gameObject.GetComponent<Mark> ();
			this.timer = new Timer ();
		}

		public override void Update() {
			this.timer.Update (Time.fixedDeltaTime);
			this.mark.ColorLert (this.mark.originColor, this.mark.targetColor, this.timer.GetProcess ());

			if (!this.timer.IsRunning ()) {
				this.statemgr.Play (this.data.nextState);
			}
		}

		public override void Enter() {
			this.mark.Play ();
			this.timer.Enter (this.mark.GetTime ());
		}
	}
}

