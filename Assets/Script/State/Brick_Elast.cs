using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.State {
	using Component;
	using Utility;
	using StateData = Data.Brick_Elast;

	public class Brick_Elast : State {
		private StateData data;
		private Brick brick;

		public Brick_Elast (GameObject gameObject, StateData data) : base (gameObject, data) {
			this.data = data;
			this.brick = gameObject.GetComponent<Brick> ();
		}

		private void GotoNextState () {
			this.statemgr.Play (this.data.nextState);
		}

		private void MovePosition (int type) {
			this.brick.MovePosition (0, this.data.positioning [type] * -this.brick.direction, this.data.time)
				.SetEase (Ease.InOutQuad);
		}

		public override void Enter() {
			float time = this.data.time;

			Sequence s = DOTween.Sequence();
			Tweener t1 = this.brick.transform.DOScaleX (this.data.scaling [1], time)
				.OnPlay (() => this.MovePosition (1));
			Tweener t2 = this.brick.transform.DOScaleX (this.data.scaling [0], time)
				.OnPlay (() => {
				this.MovePosition (0);
				this.brick.MoveColor (this.brick.targetColor, time);
			})
				.SetEase (Ease.InOutBack);

			s.Append (t1);
			s.Append (t2);
			s.AppendCallback (this.GotoNextState);

			Sound.Play (this.data.clip);
		}

		public override void OnCollisionEnter(Collision collision) {
			Ball ball = collision.gameObject.GetComponent<Ball> ();

			if (ball != null) {
				float power = collision.rigidbody.velocity.x > 0 ? this.data.power : -this.data.power;
				ball.Move (power, 0, 0);
			}
		}
	}
}

