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

        public static bool Online {
            get {
                return INSTANCE.online;
            }
        }

        private int updateTimer;
        private int frame;
        private int playFrame;
        private List<PlayData> playDataList;
        private NetworkConnection connection;
        private bool online;

        protected void Awake() {
            INSTANCE = this;

            this.playDataList = new List<PlayData>();
            Networkmgr.OnClientConnectEvent += this.OnStart;
            Networkmgr.OnStopClientEvent += this.OnStop;
            
            DOTween.defaultUpdateType = UpdateType.Manual;
            DOTween.Init();
            Client.LateUpdateEvent += () => DOTween.ManualUpdate(STDDT, STDDT);
        }
        
        /*
        protected void OnGUI() {
            GUILayout.TextField(this.playFrame.ToString());
        }
        */

        protected void Update() {
            this.updateTimer += Mathf.CeilToInt(Time.deltaTime * 1000);

            while (this.updateTimer >= DT) {
                if (this.online && (this.frame + 1) % Server.WAITTING_INTERVAL == 0 && this.playDataList.Count > 1) {
                    while (this.playDataList.Count > 0) {
                        this.LockUpdate();
                    }
                }
                else {
                    this.LockUpdate();
                }
                
                this.updateTimer -= DT;
            }
        }

        private void LockUpdate() {
            if (this.online && (this.frame + 1) % Server.WAITTING_INTERVAL == 0 && this.playDataList.Count == 0) {
                return;
            }

            if (this.online) {
                this.frame++;

                if (this.frame % Server.WAITTING_INTERVAL == 0) {
                    var data = this.playDataList[0];
                    
                    /*
                    for (int i = 0; i < data.newPlayers.Length; i++) {
                        var player = GameObject.Instantiate(playerPrefab);
                        var controller = player.GetComponent<Controller>();

                        controller.connectionId = data.newPlayers[i];
                        this.controllerMap.Add(controller.connectionId, controller);
                    }

                    for (int i = 0; i < data.connIds.Length; i++) {
                        this.controllerMap[data.connIds[i]].SetInput(data.inputDatas[i].mousePos, data.inputDatas[i].isDown);
                    }
                    */

                    this.playFrame++;
                    this.playDataList.RemoveAt(0);
                    /*
                    if (this.playDataList.Count == 0) {
                        var msg = new Message.ReceiveReport() {
                            playFrame = this.playFrame,
                            inputData = new InputData() {
                                mousePos = Client.MousePosition,
                                isDown = Input.GetMouseButton(0)
                            },
                            comparison = (this.controllerMap[0].transform.localScale.x).ToBinStr()
                        };
                        
                        this.connection.Send(CustomMsgType.ReceiveReport, msg);
                    }
                    */
                }
            }

            Client.UpdateEvent();
            Client.LateUpdateEvent();
        }

        private void OnStart(NetworkConnection conn) {
            this.connection = conn;
            this.connection.RegisterHandler(CustomMsgType.AddPlayer, this.Init);
            this.connection.RegisterHandler(CustomMsgType.AddPlayData, this.AddPlayData);
            this.connection.RegisterHandler(CustomMsgType.DelConnection, this.Disconnect);
            this.connection.Send(CustomMsgType.AddConnection, new Message.Empty());
        }

        private void OnStop() {
            Debug.LogError("stop");
            this.playDataList.Clear();
            //this.connection = null;
            this.updateTimer = 0;
            this.frame = 0;
            this.playFrame = 0;
            this.online = false;
            //Application.Quit();
        }

        private void Init(NetworkMessage netMsg) {
            Debug.LogError("init");
            //this.online = true;
        }

        private void AddPlayData(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.AddPlayData>();
            this.playDataList.Add(msg.playData);
        }

        private void Disconnect(NetworkMessage netMsg) {
            Networkmgr.ExitMatch();
        }
    }
}