using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component.Network {
    public class Server : MonoBehaviour {
        public const int WAITTING_INTERVAL = 3;
        private const int PLAYER_COUNT = 2;

        private List<NetworkConnection> connList;
        private Dictionary<int, Message.Report> reportMap;
        private Dictionary<int, Message.Report> nextReportMap;
        private int frame;
        private int playFrame;

        protected void Awake() {
            this.connList = new List<NetworkConnection>();
            this.reportMap = new Dictionary<int, Message.Report>();
            this.nextReportMap = new Dictionary<int, Message.Report>();

            Networkmgr.OnServerDisconnectEvent += this.DelConnection;
        }

        protected void OnEnable() {
            NetworkServer.RegisterHandler(CustomMsgType.AddConnection, this.AddConnection);
            NetworkServer.RegisterHandler(CustomMsgType.Report, this.ReceiveReport);
        }

        protected void OnDisable() {
            this.frame = 0;
            this.playFrame = 0;
            this.connList.Clear();
            this.reportMap.Clear();
            this.nextReportMap.Clear();
        }

        protected void FixedUpdate() {
            if (this.connList.Count < PLAYER_COUNT) {
                return;
            }
            if ((this.frame + 1) % WAITTING_INTERVAL == 0 && !this.CheckCanNext()) {
                return;
            }
            
            this.frame++;

            if (this.frame % WAITTING_INTERVAL == 0) {
                this.playFrame++;
                
                var connIds = new int[this.reportMap.Count];
                var inputDatas = new InputData[this.reportMap.Count];
                int i = 0;
                /*
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
                */

                foreach (var index in this.reportMap) {
                    connIds[i] = index.Key;
                    inputDatas[i] = index.Value.inputData;
                    i++;
                }

                var msg = new Message.PlayData() {
                    playData = new PlayData() {
                        connIds = connIds,
                        inputDatas = inputDatas
                    }
                };

                this.SendToAll(CustomMsgType.PlayData, msg);
                
                this.reportMap.Clear();
                foreach (var index in this.nextReportMap) {
                    this.reportMap.Add(index.Key, index.Value);
                }
                this.nextReportMap.Clear();
            }
        }

        private void SendToAll(short msgType, MessageBase msg) {
            for (int i = 0; i < this.connList.Count; i++) {
                this.connList[i].Send(msgType, msg);
            }
        }

        private void AddConnection(NetworkMessage netMsg) {
            this.connList.Add(netMsg.conn);
            this.reportMap.Add(netMsg.conn.connectionId, new Message.Report());

            if (this.connList.Count == PLAYER_COUNT) {
                var connIds = new int[this.connList.Count];

                for (int i = 0; i < this.connList.Count; i++) {
                    connIds[i] = this.connList[i].connectionId;
                }

                var msg = new Message.Init() {
                    seed = Time.frameCount,
                    connIds = connIds
                };

                this.SendToAll(CustomMsgType.Init, msg);
            }
        }

        private void DelConnection(NetworkConnection conn) {
            var msg = new Message.Empty();
            this.SendToAll(CustomMsgType.DelConnection, msg);
            this.gameObject.SetActive(false);
        }

        private void ReceiveReport(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.Report>(); 
            Dictionary<int, Message.Report> reportMap = null; 
            //print(msg.playFrame + "," + this.playFrame);
            if (msg.playFrame > this.playFrame) {
                reportMap = this.nextReportMap;
            }
            else if (msg.playFrame == this.playFrame) {
                reportMap = this.reportMap;
            }

            if (reportMap != null) {
                reportMap.Add(netMsg.conn.connectionId, msg);
            }
        }

        private bool CheckCanNext() {
            for (int i = 0; i < this.connList.Count; i++) {
                if (!this.reportMap.ContainsKey(this.connList[i].connectionId)) {
                    return false;
                }
            }

            return true;
        }
    }
}
