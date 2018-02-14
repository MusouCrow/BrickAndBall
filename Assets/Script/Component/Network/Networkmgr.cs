using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

namespace Game.Component.Network {
    using Utility;

    public class Networkmgr : MonoBehaviour {
        public const int PLAYER_COUNT = 2;
        private static Networkmgr INSTANCE;

        public static void Connect() {
            INSTANCE.connection.ConnectToRegionMaster("cn");
            //INSTANCE.connection.Connect("192.168.31.140:5055", null, null, null, null);
        }

        public static void Disconnect() {
            INSTANCE.connection.Disconnect();
        }

        [SerializeField]
        private string appId;
        [SerializeField]
        private string version;
        [SerializeField]
        private Slot startGameSlot;

        private Connection connection;
        private Client client;
        private Server server;

        protected void Awake() {
            INSTANCE = this;

            this.connection = new Connection();
            this.connection.AppId = this.appId;
            this.connection.AppVersion = this.version;
            this.connection.NameServerHost = "ns-cn.exitgames.com";
            this.connection.AutoJoinLobby = false;
            this.connection.OnOpResponseAction += this.OnOperationResponse;
            this.connection.OnStateChangeAction += this.OnStatusChanged;

            this.client = new Client(this.connection, this.startGameSlot);
            this.server = new Server(this.connection);
        }

        protected void Update() {
            this.connection.Service();
            this.client.Update();
        }

        protected void OnGUI() {
            GUILayout.Label(this.connection.State.ToString() + "," + this.connection.Server.ToString());
            this.client.OnGUI();
        }

        private void OnOperationResponse(OperationResponse operationResponse) {
            if (operationResponse.OperationCode == OperationCode.Authenticate) {
                if (this.connection.Server == ServerConnection.MasterServer) {
                    this.connection.OpJoinRandomRoom(null, PLAYER_COUNT);
                }
            }
            else if (operationResponse.OperationCode == OperationCode.JoinRandomGame) {
                if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound) {
                    this.connection.CreateRoom();
                }
            }
            else if (operationResponse.OperationCode == OperationCode.GetRegions) {
                for (int i = 0; i < this.connection.AvailableRegions.Length; i++) {
                    Debug.Log(this.connection.AvailableRegions[i] + ", " + this.connection.AvailableRegionsServers[i]);
                }
            }
        }

        private void OnStatusChanged(ClientState state) {
            if (state == ClientState.ConnectedToNameServer) {
                this.connection.OpGetRegions();
            }
        }
    }
}
