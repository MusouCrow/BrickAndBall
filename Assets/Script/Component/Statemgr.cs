using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = System.Object;

namespace Game.Component {
	using Utility;

	public class Statemgr : MonoBehaviour {
		[SerializeField]
		private string[] keys;
		[SerializeField]
		private State.Data[] values;

		private State nowState;
		private Dictionary<string, State> stateMap;
		private string nowStateName;

		void Start () {
			this.stateMap = new Dictionary<string, State> ();

			for (int i = 0; i < this.values.Length; i++) {
				State state = Activator.CreateInstance (this.values [i].type, new Object[] { this.gameObject, this.values [i] }) as State;
				this.stateMap.Add (this.keys [i], state);
			}

			this.keys = null;
			this.values = null;

			this.Play ("Normal");
		}

		void FixedUpdate () {
			if (this.nowState != null) {
				this.nowState.Update ();
			}
		}

		void OnMouseDown () {
			if (this.nowState != null) {
				this.nowState.OnMouseDown ();
			}
		}

		void OnMouseDrag () {
			if (this.nowState != null) {
				this.nowState.OnMouseDrag ();
			}
		}

		void OnCollisionEnter(Collision collision) {
			if (this.nowState != null) {
				this.nowState.OnCollisionEnter (collision);
			}
		}

		public void Play (string name) {
			if (this.nowState != null) {
				this.nowState.Exit ();
			}

			this.nowStateName = name;
			this.nowState = this.stateMap [name];
			this.nowState.Enter ();
		}

		public string GetStateName () {
			return this.nowStateName;
		}
	}
}