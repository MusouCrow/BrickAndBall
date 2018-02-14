using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

using PEventCode = ExitGames.Client.Photon.LoadBalancing.EventCode;

namespace Game.Component.Network {
    using Utility;

    public class Client {
        public const float STDDT = 0.017f; 
        public delegate void Delegate();
        public static event Delegate UpdateEvent;
        public static event Delegate LateUpdateEvent;
        private static Client INSTANCE;
        private const int DT = 17;
        private const int WAITTING_INTERVAL = 7;

        public static bool Online {
            get {
                return INSTANCE.online;
            }
            set {
                INSTANCE.online = value;
            }
        }

        public static Vector3 MousePosition {
            get {
                return ViceCamera.ScreenToWorldPoint(Input.mousePosition, 0.3f).ToFixed();
            }
        }

        public static int PlayFrame {
            get {
                return INSTANCE.playFrame;
            }
        }

        private Connection connection;
        private int updateTimer;
        private int frame;
        private int playFrame;
        private PlayData playData;
        private bool online;
        private Slot startGameSlot;
        private bool first;

        public Client(Connection connection, Slot startGameSlot) {
            INSTANCE = this;
            this.connection = connection;
            this.connection.AutoJoinLobby = false;
            this.startGameSlot = startGameSlot;
            this.connection.OnStateChangeAction += this.OnStatusChanged;
            this.connection.OnEventCall += this.OnEventCall;

            DOTween.defaultUpdateType = UpdateType.Manual;
            DOTween.Init();
            Client.LateUpdateEvent += () => DOTween.ManualUpdate(STDDT, STDDT);
        }

        public void OnGUI() {
            if (this.online) {
                GUILayout.Label(this.playFrame.ToString());
            }
        }

        public void Update() {
            this.updateTimer += Mathf.CeilToInt(Time.deltaTime * 1000);

            while (this.updateTimer >= DT) {
                this.LockUpdate();
                this.updateTimer -= DT;
            }
        }

        private void LockUpdate() {
            if (this.online && this.frame + 1 == WAITTING_INTERVAL && this.playData == null) {
                return;
            }

            if (this.online) {
                this.frame++;

                if (this.frame == WAITTING_INTERVAL) {
                    var data = this.playData;
                    this.playData = null;
                    
                    if (data.connIds != null) {
                        for (int i = 0; i < data.connIds.Length; i++) {
                            Judge.SetInput(data.connIds[i], data.inputDatas[i]);
                        }
                    }

                    this.playFrame++;
                    this.frame = 0;
                    
                    var sendMsg = new InputData() {
                        mousePos = new SVector3(Client.MousePosition),
                        isDown = Input.GetMouseButton(0)
                    };

                    this.connection.Send(EventCode.Report, sendMsg, true);
                    
                    /*
                    var sendMsg2 = new Message.Comparison() {
                        playFrame = this.playFrame,
                        content = Judge.GetMD5()
                    };

                    this.connection.Send(EventCode.Comparison, sendMsg2);
                    */
                }
            }

            Client.UpdateEvent();
            Client.LateUpdateEvent();
        }

        private void OnStatusChanged(ClientState state) {
            if (state == ClientState.Joined) {
                this.connection.Send(EventCode.Connect, null, true);
            }
            else if (state == ClientState.Disconnected) {
                this.online = false;

                if (Judge.GameType == GameType.PVP) {
                    Judge.GameType = GameType.PVE;
                }
            }
        }

        private void OnEventCall(byte eventCode, object content, int senderId) {
            if (eventCode == EventCode.Init) {
                var msg = this.connection.Receive<Message.Init>(content);
                
                Random.InitState(msg.seed);
                Judge.PlayerType = this.connection.LocalPlayer.ID == msg.connIds[0] ? PlayerType.A : PlayerType.B;
                this.startGameSlot.Run(null);
                this.online = true;
                this.updateTimer = 0;
                this.frame = 0;
                this.playFrame = 0;
                this.playData = new PlayData();
            }
            else if (eventCode == EventCode.PlayData) {
                var msg = this.connection.Receive<PlayData>(content);
                this.playData = msg;
            }
            else if (eventCode == PEventCode.Leave) {
                if (Judge.GameType != GameType.NONE) {
                    this.connection.Disconnect();
                }
            }
        }
    }
}
