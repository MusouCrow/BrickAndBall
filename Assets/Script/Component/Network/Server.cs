using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component.Network {
    public class Server : MonoBehaviour {
        private const int PLAYER_COUNT = 2;

        private List<NetworkConnection> connList;
        private Dictionary<int, Message.Report> reportMap;

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
            this.connList.Clear();
            this.reportMap.Clear();
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

            if (this.CheckCanNext()) {
                this.SendPlayData();
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

        private void SendPlayData() {
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

            var msg = new Message.PlayData() {
                playData = new PlayData() {
                    connIds = connIds,
                    inputDatas = inputDatas
                }
            };

            this.SendToAll(CustomMsgType.PlayData, msg);
            this.reportMap.Clear();
        }
    }
}
