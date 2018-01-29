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
		public enum Identity {
			Player,
			AI,
			Network
		}

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
		public Identity identity;
		[System.NonSerialized]
		public Statemgr statemgr;
		private Timer timer;

		public bool CanConroll {
			get {
				return this.isRunning && this.identity == Identity.Player;
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
			this.timer = new Timer (this.AIInterval);
			this.renderer = this.GetComponent<MeshRenderer>();
			this.originColor = this.renderer.material.color;
		}

		protected override void LockUpdate() {
			if (!this.isRunning || this.identity != Identity.AI) {
				return;
			}

			this.timer.Update(Client.STDDT);

			if (!this.timer.IsRunning) {
				if (this.AITickEvent != null) {
					this.AITickEvent(Judge.BallPosition);
				}

				this.timer.Enter(this.AIInterval);
			}
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
	}
}