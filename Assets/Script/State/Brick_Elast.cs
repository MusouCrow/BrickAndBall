using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.State {
	using Component;
	using Utility;

	[CreateAssetMenuAttribute(menuName="Game/State/Brick_Elast")]
	public class Brick_Elast : State.Data {
		[SerializeField]
		protected Vector2 scaling;
		[SerializeField]
		protected Vector2 positioning;
		[SerializeField]
		protected float time;
		[SerializeField]
		protected float power;
		[SerializeField]
		protected AudioClip clip;
		[SerializeField]
		protected string nextState = "Normal";

		public Vector2 Scaling {
			get {return scaling;}
		}

		public Vector2 Positioning {
			get {return positioning;}
		}

		public float Time {
			get {return time;}
		}

		public float Power {
			get {return power;}
		}

		public AudioClip Clip {
			get {return clip;}
		}

		public string NextState {
			get {return nextState;}
		}
	}

	public class Brick_ElastState : State {
		private Brick_Elast data;
		private Brick brick;

		public Brick_ElastState(GameObject gameObject, Brick_Elast data) : base(gameObject, data) {
			this.data = data;
			this.brick = gameObject.GetComponent<Brick>();
		}

		private void GotoNextState() {
			this.statemgr.Play(this.data.NextState);
		}

		private void MovePosition(int type) {
			this.brick.MovePosition(0, this.data.Positioning [type] * -this.brick.direction, this.data.Time)
				.SetEase(Ease.InOutQuad);
		}

		public override void Enter() {
			float time = this.data.Time;
			var s = DOTween.Sequence();
			var t1 = this.brick.MoveScale(0, this.data.Scaling[1], time)
				.OnPlay(() => this.MovePosition(1));
			var t2 = this.brick.MoveScale(0, this.data.Scaling[0], time)
				.OnPlay(() => {
					this.MovePosition(0);
					this.brick.MoveColor(this.brick.targetColor, time);
				})
				.SetEase(Ease.InOutBack);

			s.Append(t1);
			s.Append(t2);
			s.AppendCallback(this.GotoNextState);

			Sound.Play(this.data.Clip);
		}
		
		public override void OnCollide(Collider collider) {
			var ball = collider.GetComponent<Ball>();
			
			if (ball != null) {
				ball.Move(this.data.Power * this.brick.direction, 0, ball.Velocity.z * ball.Rate);
			}
		}
	}
}

