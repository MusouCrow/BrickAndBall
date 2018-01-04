using System;
using UnityEngine;
using DG.Tweening;

namespace Game.State {
	using Component;
	using Utility;
	using StateData = Data.Mark_Elast;

	public class Mark_Elast : State {
		private StateData data;
		private Mark mark;

		public Mark_Elast (GameObject gameObject, StateData data) : base (gameObject, data) {
			this.data = data;
			this.mark = gameObject.GetComponent<Mark> ();
		}

		private void GotoNextState () {
			this.statemgr.Play (this.data.nextState);
		}

		public override void Enter() {
			this.mark.Play ();
			this.mark.MoveColor (this.mark.targetColor, this.mark.GetTime ())
				.OnComplete (this.GotoNextState);
		}
	}
}

