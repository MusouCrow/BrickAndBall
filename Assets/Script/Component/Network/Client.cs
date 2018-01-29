using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

namespace Game.Component.Network {
    using Utility;

    public class Client : MonoBehaviour {
        public const int STDDT = 17;
        public const int STD_WIDTH = 1280;
        public const int STD_HEIGHT = 720;
        public delegate void Delegate();
        public static event Delegate LockUpdateEvent;

        private static Vector2 MousePosition {
            get {
                float x = Input.mousePosition.x * ((float)STD_WIDTH / (float)Screen.width).ToFixed();
                float y = Input.mousePosition.y * ((float)STD_HEIGHT / (float)Screen.height).ToFixed();

                return new Vector2(x, y);
            }
        }

        private int updateTimer;
        private int frame;
        private int playFrame;
        private List<PlayData> playDataList;
        private Dictionary<int, Controller> controllerMap;
        private NetworkConnection connection;
        private bool start;

        protected void Awake() {
            this.playDataList = new List<PlayData>();
            this.controllerMap = new Dictionary<int, Controller>();
            Networkmgr.OnClientConnectEvent += this.OnStart;
            Networkmgr.OnStopClientEvent += this.OnStop;
            /*
            DOTween.defaultUpdateType = UpdateType.Manual;
            DOTween.Init();
            Client.LockUpdateEvent += () => DOTween.ManualUpdate(0.017f, 0.017f);
            */
        }

        private void OnStart(NetworkConnection conn) {
            this.connection = conn;
            this.connection.RegisterHandler(CustomMsgType.AddPlayer, this.Init);
            this.connection.RegisterHandler(CustomMsgType.AddPlayData, this.AddPlayData);
            this.connection.RegisterHandler(CustomMsgType.DelConnection, this.DelPlayer);
            this.connection.Send(CustomMsgType.AddConnection, new Message.AddConnection());
        }

        private void OnStop() {
            Application.Quit();
        }
        
        /*
        protected void OnGUI() {
            GUILayout.TextField(this.playFrame.ToString());
        }
        */

        protected void Update() {
            if (!this.start) {
                return;
            }

            this.updateTimer += Mathf.CeilToInt(Time.deltaTime * 1000);

            while (this.updateTimer >= STDDT) {
                if ((this.frame + 1) % Server.WAITTING_INTERVAL == 0 && this.playDataList.Count > 1) {
                    while (this.playDataList.Count > 0) {
                        this.LockUpdate();
                    }
                }
                else {
                    this.LockUpdate();
                }
                
                this.updateTimer -= STDDT;
            }
        }

        private void LockUpdate() {
            if ((this.frame + 1) % Server.WAITTING_INTERVAL == 0 && this.playDataList.Count == 0) {
                return;
            }

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
            }

            Client.LockUpdateEvent();
        }

        private void Init(NetworkMessage netMsg) {
            this.start = true;
        }

        private void AddPlayData(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.AddPlayData>();
            this.playDataList.Add(msg.playData);
        }

        private void DelPlayer(NetworkMessage netMsg) {
            var msg = netMsg.ReadMessage<Message.DelConnection>();
            
            if (this.controllerMap.ContainsKey(msg.connectionId)) {
                GameObject.Destroy(this.controllerMap[msg.connectionId].gameObject);
                this.controllerMap.Remove(msg.connectionId);
            }
        }
    }
}