using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

namespace Game.Component.Network {
    using Utility;

    public class Client : MonoBehaviour {
        public const float STDDT = 0.017f; 
        public delegate void Delegate();
        public static event Delegate UpdateEvent;
        public static event Delegate LateUpdateEvent;
        private static Client INSTANCE;
        private const int DT = 17;
        private const int WAITTING_INTERVAL = 3;

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

        [SerializeField]
        private Slot startGameSlot;

        private int updateTimer;
        private int frame;
        private int playFrame;
        private PlayData playData;
        private NetworkConnection connection;
        private bool online;

        protected void Awake() {
            INSTANCE = this;

            Networkmgr.OnClientConnectEvent += this.OnStart;
            Networkmgr.OnStopClientEvent += this.OnStop;
            
            DOTween.defaultUpdateType = UpdateType.Manual;
            DOTween.Init();
            Client.LateUpdateEvent += () => DOTween.ManualUpdate(STDDT, STDDT);
        }
        
        protected void OnGUI() {
            if (this.online) {
                GUILayout.TextField(this.playFrame.ToString());
            }
        }

        protected void Update() {
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
                    
                    var msg = new Message.Report() {
                        //playFrame = this.playFrame,
                        inputData = new InputData() {
                            mousePos = Client.MousePosition,
                            isDown = Input.GetMouseButton(0)
                        },
                        comparison = Judge.NewPack()
                    };

                    this.connection.Send(CustomMsgType.Report, msg);
                }
            }

            Client.UpdateEvent();
            Client.LateUpdateEvent();
        }

        private void OnStart(NetworkConnection conn) {
            this.connection = conn;
            this.connection.RegisterHandler(CustomMsgType.Init, this.Init);
            this.connection.RegisterHandler(CustomMsgType.PlayData, this.ReceivePlayData);
            this.connection.RegisterHandler(CustomMsgType.DelConnection, this.Disconnect);
            this.connection.Send(CustomMsgType.AddConnection, new Message.Empty());
        }

        private void OnStop() {
            this.online = false;
            this.connection = null;

            if (Judge.GameType == GameType.PVP) {
                Judge.GameType = GameType.PVE;
            }
        }

        private void Init(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.Init>();

            Random.InitState(msg.seed);
            Judge.PlayerType = this.connection.connectionId == msg.connIds[0] ? PlayerType.A : PlayerType.B;
            this.startGameSlot.Run(this.gameObject);

            this.online = true;
            this.updateTimer = 0;
            this.frame = 0;
            this.playFrame = 0;
            this.playData = new PlayData();
        }

        private void ReceivePlayData(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.PlayData>();
            this.playData = msg.playData;
        }

        private void Disconnect(NetworkMessage netMsg) {
            Networkmgr.ExitMatch();

            if (Judge.GameType == GameType.NONE) {
                Networkmgr.StartMatch();
            }
        }
    }
}