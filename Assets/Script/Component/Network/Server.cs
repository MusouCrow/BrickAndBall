using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component.Network {
    public class Server : MonoBehaviour {
        public const int WAITTING_INTERVAL = 3;

        private List<NetworkConnection> connList;
        private List<int> newPlayerList;
        private Dictionary<int, Message.ReceiveReport> reportMap;
        private int wattingFrame;
        private int playFrame;
        private bool canNext;

        protected void Awake() {
            this.connList = new List<NetworkConnection>();
            this.newPlayerList = new List<int>();
            this.reportMap = new Dictionary<int, Message.ReceiveReport>();
            this.canNext = true;

            NetworkServer.RegisterHandler(CustomMsgType.AddConnection, this.AddConnection);
            NetworkServer.RegisterHandler(CustomMsgType.ReceiveReport, this.ReceiveReport);
            Networkmgr.OnServerDisconnectEvent += this.DelConnection;
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
                    Message.ReceiveReport late = null;

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

                    if (this.newPlayerList.Count > 0) {
                        playData.newPlayers = this.newPlayerList.ToArray();
                        this.newPlayerList.Clear();
                    }

                    this.reportMap.Clear();

                    var msg = new Message.AddPlayData() {
                        playData = playData
                    };
                    
                    this.SendToAll(CustomMsgType.AddPlayData, msg);
                }
            }
        }

        private void DelConnection(NetworkConnection conn) {
            this.connList.Remove(conn);
            
            var msg = new Message.DelConnection() {
                connectionId = conn.connectionId
            };
            
            this.SendToAll(CustomMsgType.DelConnection, msg);
            this.TryCanNext();
        }

        private void SendToAll(short msgType, MessageBase msg) {
            for (int i = 0; i < this.connList.Count; i++) {
                this.connList[i].Send(msgType, msg);
            }
        }

        private void AddConnection(NetworkMessage netMsg) {
            var id = netMsg.conn.connectionId;
            var msg = new Message.AddPlayer();
            
            netMsg.conn.Send(CustomMsgType.AddPlayer, msg);
            this.connList.Add(netMsg.conn);
            this.newPlayerList.Add(id);
        }

        private void ReceiveReport(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.ReceiveReport>(); 

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
