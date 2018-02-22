using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Network {
    using Utility;
    using Field;

    public class Networkmgr : MonoBehaviour {
        public const float STDDT = 0.017f;
        public static event Action UpdateEvent;
        public static event Action LateUpdateEvent;
        private static Networkmgr INSTANCE;
        private const int DT = 17;
        private const int WAITTING_INTERVAL = 4;

        public static Vector3 MousePosition {
            get {
                return ViceCamera.ScreenToWorldPoint(Input.mousePosition, 0.3f).ToFixed();
            }
        }

        public static bool Connect() {
            return INSTANCE.client.Connect();
        }

        public static bool Disconnect() {
            return INSTANCE.client.Disconnect();
        }

        [SerializeField]
        private Slot startGameSlot;
        [SerializeField]
        private Slot exitMatchSlot;
        private int updateTimer;
        private int frame;
        private int playFrame;
        private PlayData playData;
        private Client client;

        protected void Awake() {
            INSTANCE = this;

            this.client = new Client("127.0.0.1", 8888);
            this.client.OnConnect += this.OnConnect;
            this.client.OnDisconnect += this.OnDisconnect;

            //Networkmgr.OnClientConnectEvent += this.OnStart;
            //Networkmgr.OnStopClientEvent += this.OnStop;
            
            DOTween.defaultUpdateType = UpdateType.Manual;
            DOTween.Init();
            Networkmgr.LateUpdateEvent += () => DOTween.ManualUpdate(STDDT, STDDT);
        }

        protected void OnGUI() {
            if (this.client.Connected) {
                GUILayout.TextField(this.playFrame.ToString());
            }
        }

        protected void Update() {
            this.updateTimer += Mathf.CeilToInt(Time.deltaTime * 1000);

            while (this.updateTimer >= DT) {
                this.client.Update();
                this.LockUpdate();
                this.updateTimer -= DT;
            }
        }

        private void LockUpdate() {
            if (this.client.Connected && this.frame + 1 == WAITTING_INTERVAL && this.playData == null) {
                return;
            }

            if (this.client.Connected) {
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
                    /*
                    var msg = new Message.Report() {
                        inputData = new InputData() {
                            mousePos = Client.MousePosition,
                            isDown = Input.GetMouseButton(0)
                        }
                    };

                    this.connection.Send(CustomMsgType.Report, msg);
                    
                    var msg2 = new Message.Comparison() {
                        playFrame = this.playFrame,
                        content = Judge.GetMD5()
                    };

                    this.connection.Send(CustomMsgType.Comparison, msg2);
                    */
                }
            }

            Networkmgr.UpdateEvent();
            Networkmgr.LateUpdateEvent();
        }

        private void OnConnect() {
            /*
            this.connection = conn;
            this.connection.RegisterHandler(CustomMsgType.Init, this.Init);
            this.connection.RegisterHandler(CustomMsgType.PlayData, this.ReceivePlayData);
            this.connection.RegisterHandler(CustomMsgType.DelConnection, this.Disconnect);
            this.connection.Send(CustomMsgType.AddConnection, new Message.Empty());
            */
        }

        private void OnDisconnect() {
            if (Judge.GameType == GameType.PVP) {
                Judge.GameType = GameType.PVE;
            }
            else {
                this.exitMatchSlot.Run(this.gameObject);
            }
        }

        /*
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
        */
    }
}
