using System;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

namespace Game.Component.Network {
    public class Server {
        private Connection connection;
        private Dictionary<int, InputData> inputMap;
        private Dictionary<int, Dictionary<int, string>> comparisonMap;

        public Server(Connection connection) {
            this.connection = connection;
            this.inputMap = new Dictionary<int, InputData>();
            this.comparisonMap = new Dictionary<int, Dictionary<int, string>>();
            this.connection.OnStateChangeAction += this.OnStatusChanged;
            this.connection.OnEventCall += this.OnEventCall;
        }

        private void OnStatusChanged(ClientState state) {
            if (state == ClientState.Joined) {
                this.inputMap.Clear();
                this.comparisonMap.Clear();
            }
        }

        private void OnEventCall(byte eventCode, object content, int senderId) {
            if (eventCode == EventCode.Connect) {
                if (this.connection.CurrentRoom.Players.Count == Networkmgr.PLAYER_COUNT) {
                    var connIds = new int[Networkmgr.PLAYER_COUNT];
                    int i = 0;

                    foreach (var index in this.connection.CurrentRoom.Players) {
                        connIds[i] = index.Key;
                        i++;
                    }

                    var sendMsg = new Message.Init() {
                        seed = Time.frameCount,
                        connIds = connIds
                    };

                    this.connection.Send(EventCode.Init, sendMsg);
                }
            }
            else if (eventCode == EventCode.Report) {
                var msg = this.connection.Receive<InputData>(content);
                this.inputMap.Add(senderId, msg);

                if (this.inputMap.Count == Networkmgr.PLAYER_COUNT) {
                    var connIds = new int[this.inputMap.Count];
                    var inputDatas = new InputData[this.inputMap.Count];
                    int i = 0;

                    foreach (var index in this.inputMap) {
                        connIds[i] = index.Key;
                        inputDatas[i] = index.Value;
                        i++;
                    }

                    /*
                    var connIds = new int[Networkmgr.PLAYER_COUNT];
                    int i = 0;

                    foreach (var index in this.connection.CurrentRoom.Players) {
                        connIds[i] = index.Key;
                        i++;
                    } */

                    var sendMsg = new PlayData() {
                        connIds = connIds,
                        inputDatas = inputDatas
                    };

                    this.connection.Send(EventCode.PlayData, sendMsg);
                    this.inputMap.Clear();
                }
            }
            else if (eventCode == EventCode.Comparison) {
                var msg = this.connection.Receive<Message.Comparison>(content);

                if (!this.comparisonMap.ContainsKey(msg.playFrame)) {
                    this.comparisonMap.Add(msg.playFrame, new Dictionary<int, string>());
                }

                var map = this.comparisonMap[msg.playFrame];
                map.Add(senderId, msg.content);

                if (map.Count == Networkmgr.PLAYER_COUNT) {
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
        }
    }
}