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
        private string serverAddress;
        [SerializeField]
        private int serverPort;
        [SerializeField]
        private Slot startGameSlot;
        [SerializeField]
        private Slot exitMatchSlot;
        private int updateTimer;
        private int frame;
        private int playFrame;
        private List<PlayData> playDataList;
        private Client client;
        private bool online;
        private string addr;

        protected void Awake() {
            INSTANCE = this;

            this.playDataList = new List<PlayData>();

            this.client = new Client(this.serverAddress, this.serverPort);
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
                if (this.playDataList.Count > 1) {
                    var lateFrame = this.frame;
                    do {
                        this.LockUpdate();
                    } while(this.playDataList.Count == 0 && this.frame == lateFrame);
                }
                else {
                    this.LockUpdate();
                }
                
                this.updateTimer -= DT;
            }
        }

        private void LockUpdate() {
            this.client.Update(STDDT);

            if (this.online && this.frame + 1 == WAITTING_INTERVAL && this.playDataList.Count == 0) {
                return;
            }

            if (this.online) {
                this.frame++;

                if (this.frame == WAITTING_INTERVAL) {
                    var data = this.playDataList[0];
                    this.playDataList.RemoveAt(0);
                    
                    if (Judge.IsRunning && data.addrs != null) {
                        for (int i = 0; i < data.addrs.Length; i++) {
                            Judge.SetInput(data.addrs[i], data.inputs[i]);
                        }
                    }

                    this.playFrame++;
                    this.frame = 0;

                    var input = new Datas.Input() {
                        data = new InputData() {
                            mousePos = new SVector(Networkmgr.MousePosition),
                            isDown = Input.GetMouseButton(0)
                        },
                        frame = this.playFrame
                    };

                    this.client.Send(EventCode.Input, input);

                    var comparison = new Datas.Comparison() {
                        playFrame = this.playFrame,
                        content = Judge.Comparison
                    };

                    this.client.Send(EventCode.Comparison, comparison);
                }
            }

            Networkmgr.UpdateEvent();
            Networkmgr.LateUpdateEvent();
        }

        private void OnConnect(byte id, string data) {
            var obj = JsonUtility.FromJson<Datas.Connect>(data);
            this.addr = obj.addr;
            print("Connected");
        }

        private void OnDisconnect(byte id, string data) {
            while (this.playDataList.Count > 0) {
                this.LockUpdate();
            }
            
            this.addr = null;
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
            Judge.PlayerType = this.addr == obj.leftAddr ? PlayerType.A : PlayerType.B;
            Judge.SetAddr(obj.leftAddr, obj.rightAddr);
            this.startGameSlot.Run(this.gameObject);
            this.online = true;
            this.updateTimer = 0;
            this.frame = 0;
            this.playFrame = 0;
            this.playDataList.Clear();
            this.playDataList.Add(new PlayData());
        }

        private void OnReceivePlayData(byte id, string data) {
            this.playDataList.Add(JsonUtility.FromJson<PlayData>(data));
        }
    }
}
