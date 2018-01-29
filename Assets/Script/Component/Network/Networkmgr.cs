using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component.Network {
    public class Networkmgr : NetworkManager {
        public delegate void Delegate(NetworkConnection conn);
        public delegate void VoidDelegate();
        public static event Delegate OnClientConnectEvent;
        public static event Delegate OnServerDisconnectEvent;
        public static event VoidDelegate OnStopClientEvent;

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
    }
}
