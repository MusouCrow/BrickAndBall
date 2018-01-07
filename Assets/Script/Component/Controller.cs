using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Component {
	using Utility;

	public class Controller : MonoBehaviour {
		public delegate void ResetDelegate ();
		public delegate void AITickDelegate (Vector3 ballPosition);
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
		protected MeshRenderer renderer;
		public event ResetDelegate ResetEvent;
		public event AITickDelegate AITickEvent;
		[System.NonSerialized]
		public bool isRunning;
		[System.NonSerialized]
		public Identity identity;
		private Timer timer;

		protected void Awake () {
			this.timer = new Timer (this.GetAIInterval ());
			this.renderer = this.GetComponent<MeshRenderer> ();
			this.originColor = this.renderer.material.color;
		}

		protected void FixedUpdate () {
			if (!this.isRunning || this.identity != Identity.AI) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);

			if (!this.timer.IsRunning ()) {
				if (this.AITickEvent != null) {
					this.AITickEvent (Judge.GetBallPosition ());
				}

				this.timer.Enter (this.GetAIInterval ());
			}
		}

		private float GetAIInterval () {
			return Mathf.Lerp (this.AIIntervalRange.x, this.AIIntervalRange.y, Random.value);
		}

		public void Reset () {
			if (this.ResetEvent != null) {
				this.ResetEvent ();
			}
		}

		public Tweener MoveColor (Color value, float t) {
			return this.renderer.material.DOColor (value, t)
				.SetEase (Ease.Linear);
		}

		public bool CanConroll () {
			return this.isRunning && this.identity == Identity.Player;
		}
	}
}