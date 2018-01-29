using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = System.Object;

namespace Game.Component {
	using Utility;
	using Network;

	public class Statemgr : LockBehaviour {
		[SerializeField]
		private string[] keys;
		[SerializeField]
		private State.Data[] values;

		private State nowState;
		private Dictionary<string, State> stateMap;
		private string nowStateName;

		protected new void Awake() {
			base.Awake();

			var collider = this.GetComponent<Collider>();

			if (collider != null) {
				collider.CollisionEnterEvent += this.OnCollide;
			}

			this.stateMap = new Dictionary<string, State>();

			for (int i = 0; i < this.values.Length; i++) {
				State state = Activator.CreateInstance(this.values [i].type, new Object[] {this.gameObject, this.values [i]}) as State;
				this.stateMap.Add(this.keys [i], state);
			}

			this.keys = null;
			this.values = null;

			this.Play("Normal");
		}

		protected override void LockUpdate() {
			if (this.nowState != null) {
				this.nowState.Update();
			}
		}

		protected void OnMouseDown() {
			if (this.nowState != null) {
				this.nowState.OnMouseDown();
			}
		}

		protected void OnMouseDrag() {
			if (this.nowState != null) {
				this.nowState.OnMouseDrag();
			}
		}

		protected void OnDrawGizmosSelected() {
			if (this.nowState != null) {
				this.nowState.OnDrawGizmosSelected();
			}
		}

		public void Play(string name) {
			if (this.nowState != null) {
				this.nowState.Exit();
			}

			this.nowStateName = name;
			this.nowState = this.stateMap[name];
			this.nowState.Enter();
		}

		public string GetStateName() {
			return this.nowStateName;
		}

		public bool CheckRunning(State state) {
			return this.nowState == state;
		}

		private void OnCollide(Collider collider) {
			if (this.nowState != null) {
				this.nowState.OnCollide(collider);
			}
		}
	}
}