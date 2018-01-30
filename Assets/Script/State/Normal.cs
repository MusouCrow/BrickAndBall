using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.State {
	using Component;
	using Utility;

	[CreateAssetMenuAttribute(menuName="Game/State/Normal")]
	public class Normal : State.Data {
		[SerializeField]
		protected float coolDownTime;
		[SerializeField]
		protected AudioClip clip;

		public float CoolDownTime {
			get {return coolDownTime;}
		}

		public AudioClip Clip {
			get {return clip;}
		}
	}

	public class NormalState : State {
		private const float DRAG_DELTA = 0.3f;

		private Normal data;
		private Controller controller;
		private float clickTime;
		private bool coolDown;
		private Vector3 draggingPos;
		private Sequence sequence;
		private bool willReset;
		private Bounds bounds;

		public NormalState(GameObject gameObject, Normal data) : base(gameObject, data) {
			this.data = data;
			this.controller = gameObject.GetComponent<Controller>();
			this.coolDown = false;
			this.controller.ResetEvent += this.Reset;
			this.controller.AITickEvent += this.Elast;

			var bounds = this.gameObject.GetComponent<MeshRenderer>().bounds;
			var center = bounds.center;
			var size = bounds.size;
			center.x += bounds.size.x * this.controller.direction;
			size.y = 1;

			this.bounds = new Bounds(center, size);

			if (this.controller is Brick) {
				var brick = this.controller as Brick;
				brick.AdjustPositionEvent += this.AdjustBounds;
			}
		}

		private void AdjustBounds(Vector3 pos) {
			var center = this.bounds.center;
			center.z = pos.z;
			this.bounds.center = center;
		}

		private void Elast(Vector3 ballPosition) {
			if (!this.statemgr.CheckRunning(this) || this.coolDown) {
				return;
			}

			if (this.bounds.Contains(ballPosition)) {
				this.statemgr.Play("Elast");
			}
		}

		private void PlaySound() {
			Sound.Play(this.data.Clip);
		}

		private void CoolDownEnd() {
			this.coolDown = false;
			this.draggingPos = ViceCamera.ScreenToWorldPoint(Input.mousePosition);
		}

		private void Reset() {
			if (this.statemgr.CheckRunning(this)) {
				if (this.sequence != null) {
					this.sequence.Complete(false);
				}

				this.coolDown = false;
			} else {
				this.willReset = true;
			}
		}

		public override void Enter() {
			if (!this.coolDown) {
				return;
			}

			float coolDownTime = this.data.CoolDownTime;

			this.sequence = DOTween.Sequence();
			Tweener t1 = this.controller.MoveColor(this.controller.originColor, coolDownTime * 0.9f);
			Tweener t2 = this.controller.MoveColor(Color.white, coolDownTime * 0.05f);
			Tweener t3 = this.controller.MoveColor(this.controller.originColor, coolDownTime * 0.05f);
			
			this.sequence.Append(t1);
			this.sequence.AppendCallback(this.PlaySound);
			this.sequence.Append(t2);
			this.sequence.Append(t3);
			this.sequence.AppendCallback(this.CoolDownEnd);

			if (this.willReset) {
				this.Reset();
			}
		}

		public override void Exit() {
			this.coolDown = true;
		}

		public override void OnMouseDown() {
			if (this.coolDown || !this.controller.CanConroll) {
				return;
			}

			if (this.controller is Mark) {
				this.statemgr.Play("Elast");
			}

			this.draggingPos = ViceCamera.ScreenToWorldPoint(Input.mousePosition);
		}

		public override void OnMouseDrag() {
			if (this.controller is Mark || this.coolDown || !this.controller.CanConroll) {
				return;
			}

			Vector3 oldPos = this.draggingPos;
			this.draggingPos = ViceCamera.ScreenToWorldPoint(Input.mousePosition);
			Vector3 newPos = this.draggingPos;
			float delta = newPos.x - oldPos.x;

			if ((this.controller.direction > 0 && delta > DRAG_DELTA) || (this.controller.direction < 0 && delta < -DRAG_DELTA)) {
				this.statemgr.Play("Elast");
			}
		}

		public override void OnDrawGizmosSelected() {
			Gizmos.DrawCube(this.bounds.center, this.bounds.size);
		}
	}
}

