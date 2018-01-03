using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.State {
	using Component;
	using Utility;
	using StateData = Data.Normal;

	public class Normal : State {
		private StateData data;
		private Controller controller;
		private float clickTime;
		private float dragValue;
		private bool coolDown;
		private Vector3 draggingPos;

		public Normal (GameObject gameObject, StateData data) : base (gameObject, data) {
			this.data = data;
			this.controller = gameObject.GetComponent<Controller> ();
			this.dragValue = 0.5f;
			this.coolDown = false;
		}

		private void PlaySound () {
			Sound.Play (this.data.clip);
		}

		private void CoolDownEnd () {
			this.coolDown = false;
			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
		}

		public override void Enter() {
			if (!this.coolDown) {
				return;
			}

			float coolDownTime = this.data.coolDownTime;

			Sequence s = DOTween.Sequence();
			Tweener t1 = this.controller.MoveColor (this.controller.originColor, coolDownTime * 0.9f).SetEase (Ease.Linear);
			Tweener t2 = this.controller.MoveColor (Color.white, coolDownTime * 0.05f).SetEase (Ease.Linear);
			Tweener t3 = this.controller.MoveColor (this.controller.originColor, coolDownTime * 0.05f).SetEase (Ease.Linear);

			s.Append (t1);
			s.AppendCallback (this.PlaySound);
			s.Append (t2);
			s.Append (t3);
			s.AppendCallback (this.CoolDownEnd);
		}

		public override void Exit() {
			this.coolDown = true;
		}

		public override void OnMouseDown () {
			if (this.coolDown || !this.controller.canControll) {
				return;
			}

			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
		}

		public override void OnMouseDrag () {
			if (this.coolDown || !this.controller.canControll) {
				return;
			}

			Vector3 oldPos = this.draggingPos;
			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = this.draggingPos;
			float delta = newPos.x - oldPos.x;

			if ((this.controller.direction > 0 && delta > this.dragValue) || (this.controller.direction < 0 && delta < -this.dragValue)) {
				this.statemgr.Play ("Elast");
			}
		}
	}
}

