using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;
	using Network;

	public class Controller : LockBehaviour {
		public delegate void ResetDelegate();
		public delegate void AITickDelegate(Vector3 ballPosition);

		public Color targetColor;
		public int direction;
		[SerializeField]
		private Vector2 AIIntervalRange;

		[System.NonSerialized]
		public Color originColor;
		protected new MeshRenderer renderer;
		public event ResetDelegate ResetEvent;
		public event AITickDelegate AITickEvent;
		[System.NonSerialized]
		public bool isRunning;
		[System.NonSerialized]
		public bool isPlayer;
		[System.NonSerialized]
		public Statemgr statemgr;
		private Timer timer;

		public bool CanConroll {
			get {
				return this.isRunning && this.isPlayer;
			}
		}

		public float AIInterval {
			get {
				return Math.Lerp(this.AIIntervalRange.x, this.AIIntervalRange.y, Random.value);
			}
		}

		protected new void Awake() {
			base.Awake();

			this.statemgr = this.GetComponent<Statemgr>();
			this.renderer = this.GetComponent<MeshRenderer>();
			this.originColor = this.renderer.material.color;

			this.timer = new Timer ();
			this.timer.Enter(this.AIInterval, this.TickAI);
		}

		protected override void LockUpdate() {
			if (!this.isRunning || Judge.GameType != GameType.PVE || this.isPlayer) {
				return;
			}
			
			this.timer.Update();
		}

		public void Reset() {
			if (this.ResetEvent != null) {
				this.ResetEvent();
			}
		}

		public Tweener MoveColor(Color value, float t) {
			return this.renderer.material.DOColor(value, t.ToFixed())
				.SetEase(Ease.Linear);
		}

		private void TickAI() {
			if (this.AITickEvent != null) {
				this.AITickEvent(Judge.BallPosition);
			}

			this.timer.Enter(this.AIInterval, this.TickAI);
		}
	}
}