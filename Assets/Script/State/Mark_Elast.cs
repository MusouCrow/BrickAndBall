using System;
using UnityEngine;

namespace Game.State {
	using Component;
	using Utility;
	using Utility.SimpleJSON;

	public class Mark_Elast : State {
		private Mark mark;
		private Timer timer;
		private string nextState;

		public Mark_Elast (GameObject gameObject, JSONNode param) : base (gameObject, param) {
			this.mark = gameObject.GetComponent<Mark> ();
			this.timer = new Timer ();
			this.nextState = param ["nextState"].Value;
		}

		public override void Update() {
			this.timer.Update (Time.fixedDeltaTime);
			this.mark.ColorLert (this.mark.originColor, this.mark.targetColor, this.timer.GetProcess ());

			if (!this.timer.isRunning) {
				this.statemgr.Play (this.nextState);
			}
		}

		public override void Enter() {
			this.mark.Play ();
			this.timer.Enter (this.mark.GetTime ());
		}
	}
}

