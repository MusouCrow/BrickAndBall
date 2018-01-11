using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component {
	public class NetworkAgent : NetworkBehaviour {
		private static NetworkAgent INSTANCE;

		public static void Gain (Vector3 position) {
			INSTANCE.RpcGain (position);
		}

		protected void Awake () {
			INSTANCE = this;
		}

		[ClientRpc]
		private void RpcGain (Vector3 position) {
			Judge.Gain (position);
		}
	}
}