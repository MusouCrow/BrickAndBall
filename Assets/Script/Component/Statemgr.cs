using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = System.Object;

namespace Game.Component {
	using Utility;
	using Utility.SimpleJSON;

	public class Statemgr : MonoBehaviour {
		public string[] states;

		private State nowState;
		private Dictionary<string, State> stateMap;
		private string nowStateName;

		static private Dictionary<string, JSONNode> POOR = new Dictionary<string, JSONNode> ();

		private static JSONNode LoadJson(string path) {
			if (!POOR.ContainsKey (path)) {
				TextAsset textAsset = Resources.Load ("State/" + path, typeof(TextAsset)) as TextAsset;
				JSONNode node = JSON.Parse (textAsset.text);
				POOR.Add (path, node);
				Resources.UnloadAsset (textAsset);
			}

			return POOR [path];
		}

		void Start () {
			this.stateMap = new Dictionary<string, State> ();

			for (int i = 0; i < this.states.Length; i++) {
				string[] part = this.states [i].Split (':');
				string name = part [0];
				string path = part [1];

				JSONNode node = Statemgr.LoadJson (path);
				string script = node ["script"].Value;
				Type type = Type.GetType ("Game.State." + script);
				State state = Activator.CreateInstance (type, new Object[] { this.gameObject, node }) as State;
				
				this.stateMap.Add (name, state);
			}

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