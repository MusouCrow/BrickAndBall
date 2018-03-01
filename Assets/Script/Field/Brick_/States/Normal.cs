using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Field.Brick_.States {
	using Utility;
	using Network;

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
		private const float DRAG_DELTA = 0.4f;

		private Normal data;
		private Brick brick;
		private float clickTime;
		private bool coolDown;
		private Vector3 draggingPos;
		private Sequence sequence;
		private bool willReset;
		private Bounds bounds;

		public NormalState(GameObject gameObject, Normal data) : base(gameObject, data) {
			this.data = data;
			this.brick = gameObject.GetComponent<Brick>();
			this.brick.ResetEvent += this.Reset;
			this.brick.AITickEvent += this.Elast;
			this.brick.AdjustPositionEvent += this.AdjustBounds;

			var bounds = this.gameObject.GetComponent<MeshRenderer>().bounds;
			var center = bounds.center;
			var size = bounds.size;
			center.x += bounds.size.x * this.brick.direction;
			size.x *= 2;
			size.y = 1;

			this.bounds = new Bounds(center, size);
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
			
			float coolDownTime = (this.data.CoolDownTime / Judge.Rate).ToFixed();

			this.sequence = DOTween.Sequence();
			Tweener t1 = this.brick.MoveColor(this.brick.originColor, coolDownTime * 0.9f);
			Tweener t2 = this.brick.MoveColor(Color.white, coolDownTime * 0.05f);
			Tweener t3 = this.brick.MoveColor(this.brick.originColor, coolDownTime * 0.05f);
			
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

		public override void OnDrag(Vector3 oldPos, Vector3 newPos) {
			if (this.coolDown) {
				return;
			}
			
			float delta = newPos.x - oldPos.x;

			if ((this.brick.direction > 0 && delta > DRAG_DELTA) || (this.brick.direction < 0 && delta < -DRAG_DELTA)) {
				if (Judge.GameType == GameType.PVE) {
					this.statemgr.Play("Elast");
				}
				else {
					Networkmgr.WillElaste = true;
				}
			}
		}

		public override void OnDrawGizmosSelected() {
			Gizmos.DrawCube(this.bounds.center, this.bounds.size);
		}
	}
}

