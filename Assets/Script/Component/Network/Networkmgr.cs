using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

namespace Game.Component.Network {
    public class Networkmgr : NetworkManager {
        public delegate void Delegate(NetworkConnection conn);
        public delegate void VoidDelegate();
        public static event Delegate OnClientConnectEvent;
        public static event Delegate OnServerDisconnectEvent;
        public static event VoidDelegate OnStopClientEvent;
        private static Networkmgr INSTANCE;

        public static void StartMatch() {
			INSTANCE.StartMatchMaker();
			INSTANCE.ListMatches();
		}

		public static void ExitMatch() {
			INSTANCE.StopMatchMaker();
			INSTANCE.StopHost();
		}

		public static void Disconnect() {
			INSTANCE.StopHost();
		}

        public override void OnClientConnect(NetworkConnection conn) {
            base.OnClientConnect(conn);

            if (OnClientConnectEvent != null) {
                OnClientConnectEvent(conn);
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            base.OnServerDisconnect(conn);

            if (OnServerDisconnectEvent != null) {
                OnServerDisconnectEvent(conn);
            }
        }

        public override void OnStopClient() {
            base.OnStopClient();

            if (OnStopClientEvent != null) {
                OnStopClientEvent();
            }
        }

		public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
			base.OnMatchList(success, extendedInfo, matches);

			if (success && matches.Count > 0) {
				this.JoinMatch(matches [0].networkId);
			} else {
				this.CreateMatch();
			}
		}

		protected void Start() {
			INSTANCE = this;
		}

		private void ListMatches() {
			this.matchMaker.ListMatches(0, 10, "", true, 0, 0, this.OnMatchList);
		}

		private void JoinMatch(NetworkID netId) {
			this.matchMaker.JoinMatch(netId, "", "", "", 0, 0, this.OnMatchJoined);
		}

		private void CreateMatch() {
			this.matchMaker.CreateMatch(Time.time.ToString (), 2, true, "", "", "", 0, 0, this.OnMatchCreate);
		}
    }
}
