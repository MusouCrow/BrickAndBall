using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game.Utility {
	using Component;

	public class State {
		public class Data : ScriptableObject {
			[SerializeField]
			protected MonoScript _stateScript;

			public Type type {
				get {return _stateScript.GetClass ();}
			}
		}

		protected GameObject gameObject;
		protected Statemgr statemgr;

		public State (GameObject gameObject, Data data) {
			this.gameObject = gameObject;
			this.statemgr = this.gameObject.GetComponent<Statemgr> ();
		}

		public virtual void Update () {}
		public virtual void Enter () {}
		public virtual void Exit () {}
		public virtual void OnMouseDown () {}
		public virtual void OnMouseDrag () {}
		public virtual void OnCollisionEnter (Collision collision) {}
		public virtual void OnDrawGizmosSelected () {}
	}
}