using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Random = UnityEngine.Random;

namespace Game.Network {
    using Utility;
    using Field;

    public class Networkmgr : MonoBehaviour {
        public const float STDDT = 0.017f;
        public static event Action UpdateEvent;
        public static event Action LateUpdateEvent;
        private static Networkmgr INSTANCE;
        private const int DT = 17;
        private const int WAITTING_INTERVAL = 5;

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
        private string address;
        [SerializeField]
        private int port;
        [SerializeField]
        private Slot startGameSlot;
        [SerializeField]
        private Slot exitMatchSlot;
        private int updateTimer;
        private int frame;
        private int playFrame;
        private PlayData playData;
        private Client client;
        private bool online;
        private string fd;

        protected void Awake() {
            INSTANCE = this;

            this.client = new Client(this.address, this.port);
            this.client.RegisterEvent(EventCode.Connect, this.OnConnect);
            this.client.RegisterEvent(EventCode.Disconnect, this.OnDisconnect);
            this.client.RegisterEvent(EventCode.Start, this.OnStart);
            this.client.RegisterEvent(EventCode.Input, this.OnReceivePlayData);
            
            DOTween.defaultUpdateType = UpdateType.Manual;
            DOTween.Init();
            Networkmgr.LateUpdateEvent += () => DOTween.ManualUpdate(STDDT, STDDT);
        }

        protected void OnGUI() {
            if (this.online) {
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
            if (this.online && this.frame + 1 == WAITTING_INTERVAL && this.playData == null) {
                return;
            }

            if (this.online) {
                this.frame++;

                if (this.frame == WAITTING_INTERVAL) {
                    var data = this.playData;
                    this.playData = null;
                    
                    if (data.fds != null) {
                        for (int i = 0; i < data.fds.Length; i++) {
                            Judge.SetInput(data.fds[i], data.inputDatas[i]);
                        }
                    }

                    this.playFrame++;
                    this.frame = 0;

                    var inputData = new InputData() {
                        mousePos = new SVector(Networkmgr.MousePosition),
                        isDown = Input.GetMouseButton(0)
                    };

                    this.client.Send(EventCode.Input, inputData);
                    /*
                    var comparison = new Datas.Comparison() {
                        playFrame = this.playFrame,
                        content = Judge.Comparison
                    };

                    this.client.Send(EventCode.Comparison, comparison); */
                }
            }

            Networkmgr.UpdateEvent();
            Networkmgr.LateUpdateEvent();
        }

        private void OnConnect(byte id, string data) {
            var obj = JsonUtility.FromJson<Datas.Connect>(data);
            this.fd = obj.fd;
            print("Connected");
        }

        private void OnDisconnect(byte id, string data) {
            this.fd = null;
            this.online = false;
            
            if (Judge.GameType == GameType.PVP) {
                Judge.GameType = GameType.PVE;
            }
            else {
                this.exitMatchSlot.Run(this.gameObject);
            }

            print("Disconnected");
        }

        private void OnStart(byte id, string data) {
            var obj = JsonUtility.FromJson<Datas.Start>(data);
            
            Random.InitState(obj.seed);
            Judge.PlayerType = this.fd == obj.leftFd ? PlayerType.A : PlayerType.B;
            Judge.SetFd(obj.leftFd, obj.rightFd);
            this.startGameSlot.Run(this.gameObject);
            this.online = true;
            this.updateTimer = 0;
            this.frame = 0;
            this.playFrame = 0;
            this.playData = new PlayData();
        }

        private void OnReceivePlayData(byte id, string data) {
            this.playData = JsonUtility.FromJson<PlayData>(data);
        }
    }
}
