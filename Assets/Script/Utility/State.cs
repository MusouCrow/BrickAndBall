using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utility {
	using Component;

	public class State {
		protected GameObject gameObject;
		protected Statemgr statemgr;

		public State (GameObject gameObject, SimpleJSON.JSONNode param) {
			this.gameObject = gameObject;
			this.statemgr = this.gameObject.GetComponent<Statemgr> ();
		}

		public virtual void Update() {}
		public virtual void Enter() {}
		public virtual void Exit() {}
		public virtual void OnMouseDown() {}
		public virtual void OnMouseDrag() {}
		public virtual void OnCollisionEnter(Collision collision) {}
	}
}