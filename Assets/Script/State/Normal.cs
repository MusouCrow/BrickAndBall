using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.State {
	using Component;
	using Utility;
	using StateData = Data.Normal;

	public class Normal : State {
		private const float DRAG_DELTA = 0.3f;

		private StateData data;
		private Controller controller;
		private float clickTime;
		private bool coolDown;
		private Vector3 draggingPos;
		private Sequence sequence;
		private bool willReset;

		public Normal (GameObject gameObject, StateData data) : base (gameObject, data) {
			this.data = data;
			this.controller = gameObject.GetComponent<Controller> ();
			this.coolDown = false;
			this.controller.ResetEvent += this.Reset;
		}

		private void PlaySound () {
			Sound.Play (this.data.clip);
		}

		private void CoolDownEnd () {
			this.coolDown = false;
			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
		}

		private void Reset () {
			if (this.statemgr.CheckRunning (this)) {
				if (this.sequence != null) {
					this.sequence.Complete (false);
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

			float coolDownTime = this.data.coolDownTime;

			this.sequence = DOTween.Sequence();
			Tweener t1 = this.controller.MoveColor (this.controller.originColor, coolDownTime * 0.9f);
			Tweener t2 = this.controller.MoveColor (Color.white, coolDownTime * 0.05f);
			Tweener t3 = this.controller.MoveColor (this.controller.originColor, coolDownTime * 0.05f);
			
			this.sequence.Append (t1);
			this.sequence.AppendCallback (this.PlaySound);
			this.sequence.Append (t2);
			this.sequence.Append (t3);
			this.sequence.AppendCallback (this.CoolDownEnd);

			if (this.willReset) {
				this.Reset ();
			}
		}

		public override void Exit() {
			this.coolDown = true;
		}

		public override void OnMouseDown () {
			if (this.coolDown || !this.controller.CanConroll ()) {
				return;
			}

			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
		}

		public override void OnMouseDrag () {
			if (this.coolDown || !this.controller.CanConroll ()) {
				return;
			}

			Vector3 oldPos = this.draggingPos;
			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = this.draggingPos;
			float delta = newPos.x - oldPos.x;

			if ((this.controller.direction > 0 && delta > DRAG_DELTA) || (this.controller.direction < 0 && delta < -DRAG_DELTA)) {
				this.statemgr.Play ("Elast");
			}
		}
	}
}

