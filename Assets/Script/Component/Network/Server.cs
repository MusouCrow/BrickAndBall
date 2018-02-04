using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component.Network {
    public class Server : MonoBehaviour {
        public const int WAITTING_INTERVAL = 3;

        private List<NetworkConnection> connList;
        private Dictionary<int, Message.Report> reportMap;
        private int wattingFrame;
        private int playFrame;
        private bool canNext;

        protected void Awake() {
            this.connList = new List<NetworkConnection>();
            this.reportMap = new Dictionary<int, Message.Report>();

            Networkmgr.OnServerDisconnectEvent += this.DelConnection;
        }

        protected void OnEnable() {
            NetworkServer.RegisterHandler(CustomMsgType.AddConnection, this.AddConnection);
            NetworkServer.RegisterHandler(CustomMsgType.Report, this.ReceiveReport);
        }

        protected void OnDisable() {
            this.wattingFrame = 0;
            this.playFrame = 0;
            this.connList.Clear();
            this.reportMap.Clear();
        }

        protected void Update() {
            if (this.canNext && this.connList.Count > 0) {
                this.wattingFrame++;

                if (this.wattingFrame >= WAITTING_INTERVAL) {
                    this.playFrame++;
                    this.wattingFrame = 0;
                    this.canNext = false;
                    
                    var connIds = new int[this.reportMap.Count];
                    var inputDatas = new InputData[this.reportMap.Count];
                    int i = 0;
                    Message.Report late = null;

                    foreach (var index in this.reportMap) {
                        if (late != null) {
                            var a = index.Value.comparison;
                            var b = late.comparison;

                            if (a != b) {
                                Debug.LogError(a + " != " + b);
                            }
                        }

                        late = index.Value;
                    }

                    foreach (var index in this.reportMap) {
                        connIds[i] = index.Key;
                        inputDatas[i] = index.Value.inputData;
                        i++;
                    }

                    var playData = new PlayData() {
                        frame = playFrame,
                        connIds = connIds,
                        inputDatas = inputDatas
                    };

                    this.reportMap.Clear();

                    var msg = new Message.PlayData() {
                        playData = playData
                    };
                    
                    this.SendToAll(CustomMsgType.PlayData, msg);
                }
            }
        }

        private void SendToAll(short msgType, MessageBase msg) {
            for (int i = 0; i < this.connList.Count; i++) {
                this.connList[i].Send(msgType, msg);
            }
        }

        private void AddConnection(NetworkMessage netMsg) {
            this.connList.Add(netMsg.conn);

            if (this.connList.Count == 2) {
                var connIds = new int[this.connList.Count];

                for (int i = 0; i < this.connList.Count; i++) {
                    connIds[i] = this.connList[i].connectionId;
                }

                var msg = new Message.Init() {
                    seed = Time.frameCount,
                    connIds = connIds
                };

                this.SendToAll(CustomMsgType.Init, msg);
                this.canNext = true;
            }
        }

        private void DelConnection(NetworkConnection conn) {
            var msg = new Message.Empty();
            this.SendToAll(CustomMsgType.DelConnection, msg);
            this.gameObject.SetActive(false);
        }

        private void ReceiveReport(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.Report>(); 
            
            if (!this.canNext && msg.playFrame == this.playFrame) {
                this.reportMap.Add(netMsg.conn.connectionId, msg);
                this.TryCanNext();
            }
        }

        private void TryCanNext() {
            for (int i = 0; i < this.connList.Count; i++) {
                if (!this.reportMap.ContainsKey(this.connList[i].connectionId)) {
                    return;
                }
            }
            
            this.canNext = true;
        }
    }
}
