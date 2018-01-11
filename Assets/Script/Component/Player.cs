using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component {
	using Utility;

	public class Player : NetworkBehaviour {
		private static Slot ON_START_SLOT;

		private static void Preload () {
			ON_START_SLOT = ON_START_SLOT == null ? Resources.Load ("Slot/StartPVP") as Slot : ON_START_SLOT;
		}

		private Judge.Team team;

		protected void Awake () {
			Player.Preload ();

			bool isFull = false;
			this.team = Judge.AssignTeam (this, ref isFull);
			this.transform.localPosition = this.team.brick.transform.localPosition;

			if (isFull) {
				ON_START_SLOT.Run (this.gameObject);
			}
		}

		/*
		protected void OnDestroy () {
			if (this.team != null) {
				this.team.UnloadPlayer ();
			}
		}
		*/

		[Command]
		public void CmdPlayState (string type, string name) {
			this.RpcPlayState (type, name);
		}

		[ClientRpc]
		private void RpcPlayState (string type, string name) {
			if (type == "Game.Component.Brick") {
				this.team.brick.statemgr.Play (name);
			} else if (type == "Game.Component.Mark") {
				this.team.mark.statemgr.Play (name);
			}
		}
	}
}

