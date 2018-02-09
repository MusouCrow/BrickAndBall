using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component.Network {
    public class Server : MonoBehaviour {
        private const int PLAYER_COUNT = 2;

        private List<NetworkConnection> connList;
        private Dictionary<int, Message.Report> reportMap;
        private Dictionary<int, Dictionary<int, string>> comparisonMap;

        protected void Awake() {
            this.connList = new List<NetworkConnection>();
            this.reportMap = new Dictionary<int, Message.Report>();
            this.comparisonMap = new Dictionary<int, Dictionary<int, string>>();

            Networkmgr.OnServerDisconnectEvent += this.DelConnection;
        }

        protected void OnEnable() {
            NetworkServer.RegisterHandler(CustomMsgType.AddConnection, this.AddConnection);
            NetworkServer.RegisterHandler(CustomMsgType.Report, this.ReceiveReport);
            NetworkServer.RegisterHandler(CustomMsgType.Comparison, this.ReceiveComparison);
        }

        protected void OnDisable() {
            this.connList.Clear();
            this.reportMap.Clear();
            this.comparisonMap.Clear();
        }

        private void SendToAll(short msgType, MessageBase msg) {
            for (int i = 0; i < this.connList.Count; i++) {
                this.connList[i].Send(msgType, msg);
            }
        }

        private void AddConnection(NetworkMessage netMsg) {
            this.connList.Add(netMsg.conn);

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
            this.reportMap.Add(netMsg.conn.connectionId, msg);

            if (this.reportMap.Count == PLAYER_COUNT) {
                var connIds = new int[this.reportMap.Count];
                var inputDatas = new InputData[this.reportMap.Count];
                int i = 0;

                foreach (var index in this.reportMap) {
                    connIds[i] = index.Key;
                    inputDatas[i] = index.Value.inputData;
                    i++;
                }

                var playDataMsg = new Message.PlayData() {
                    playData = new PlayData() {
                        connIds = connIds,
                        inputDatas = inputDatas
                    }
                };

                this.SendToAll(CustomMsgType.PlayData, playDataMsg);
                this.reportMap.Clear();
            }
        }

        private void ReceiveComparison(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.Comparison>();
            
            if (!this.comparisonMap.ContainsKey(msg.playFrame)) {
                this.comparisonMap.Add(msg.playFrame, new Dictionary<int, string>());
            }

            var map = this.comparisonMap[msg.playFrame];
            map.Add(netMsg.conn.connectionId, msg.content);

            if (map.Count == PLAYER_COUNT) {
                string late = null;

                foreach (var index in map) {
                    if (late != null) {
                        var a = index.Value;
                        var b = late;

                        if (a != b) {
                            Debug.LogError(a + " != " + b);
                        }
                    }

                    late = index.Value;
                }

                this.comparisonMap.Remove(msg.playFrame);
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
