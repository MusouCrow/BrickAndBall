using System;
using UnityEngine;
using DG.Tweening;

namespace Game.State {
	using Utility;
	using Component;

	[CreateAssetMenuAttribute(menuName="Game/State/Mark_Elast")]
	public class Mark_Elast : State.Data {
		[SerializeField]
		protected string nextState = "Normal";

		public string NextState {
			get {return nextState;}
		}
	}

	public class Mark_ElastState : State {
		private Mark_Elast data;
		private Mark mark;

		public Mark_ElastState(GameObject gameObject, Mark_Elast data) : base(gameObject, data) {
			this.data = data;
			this.mark = gameObject.GetComponent<Mark>();
		}

		private void GotoNextState() {
			this.statemgr.Play(this.data.NextState);
		}

		public override void Enter() {
			this.mark.Play();
			this.mark.MoveColor(this.mark.targetColor, this.mark.GetTime())
				.OnComplete(this.GotoNextState);
		}
	}
}

