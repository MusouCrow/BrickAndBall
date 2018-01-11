using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

namespace Game.Component {
	public class Networkmgr : NetworkManager {
		private static Networkmgr INSTANCE;

		public static void StartMatch () {
			INSTANCE.StartMatchMaker ();
			INSTANCE.ListMatches ();
		}

		public static void ExitMatch () {
			INSTANCE.StopMatchMaker ();
			INSTANCE.StopHost ();
		}

		public static void Disconnect () {
			INSTANCE.StopHost ();
		}

		protected void Start () {
			INSTANCE = this;
		}

		private void ListMatches () {
			this.matchMaker.ListMatches (0, 10, "", true, 0, 0, this.OnMatchList);
		}

		private void JoinMatch (NetworkID netId) {
			this.matchMaker.JoinMatch (netId, "", "", "", 0, 0, this.OnMatchJoined);
		}

		private void CreateMatch () {
			this.matchMaker.CreateMatch (Time.time.ToString (), 2, true, "", "", "", 0, 0, this.OnMatchCreate);
		}

		public override void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
			base.OnMatchList (success, extendedInfo, matches);

			if (success && matches.Count > 0) {
				this.JoinMatch (matches [0].networkId);
			} else {
				this.CreateMatch ();
			}
		}
	}
}