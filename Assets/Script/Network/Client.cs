using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Game.Network {
    using Utility;

    public class Client {
        private struct Packet {
            public byte id;
            public string data;
        }

        private static IPEndPoint EP;
        private const float HEARTBEAT_INTERVAL = 3;

        private string addr;
        private int port;
        private UdpClient udp;
        private KCP kcp;
        private float updateTime;
        private Timer heartbeatTimer;
        private bool heartbeat = true;
        private bool willDisconnect;
        private Dictionary<byte, List<Action<byte, string>>> eventHandler;

        public bool Connected {
            get;
            private set;
        }

        public Client(string addr, int port) {
            this.addr = addr;
            this.port = port;
            this.heartbeatTimer = new Timer();
            this.eventHandler = new Dictionary<byte, List<Action<byte, string>>>();
        }

        public void Update() {
            if (!this.Connected) {
                return;
            }

            if (this.willDisconnect) {
                this.Disconnect();
                this.willDisconnect = false;

                return;
            }

            this.updateTime += Networkmgr.STDDT;
            this.kcp.Update((uint)Mathf.FloorToInt(this.updateTime * 1000));
            this.heartbeatTimer.Update();

            for (var size = this.kcp.PeekSize(); size > 0; size = this.kcp.PeekSize()) {
                var buffer = new byte[size];

                if (this.kcp.Recv(buffer) > 0) {
                    var packet = this.Recv(buffer);

                    if (packet.id == EventCode.Connect) {
                        this.heartbeatTimer.Enter(HEARTBEAT_INTERVAL, this.HeartbeatTick);
                    }

                    this.SendEvent(packet.id, packet.data);
                    //Debug.Log(packet.id);
                }
            }
        }

        public void Send(byte id, object obj=null) {
            byte[] buffer;

            if (obj != null) {
                var data = JsonUtility.ToJson(obj);
                buffer = new byte[Encoding.UTF8.GetByteCount(data) + 1];
                buffer[0] = id;
                Encoding.UTF8.GetBytes(data, 0, data.Length, buffer, 1);
            }
            else {
                buffer = new byte[] {id};
            }
            
            this.kcp.Send(buffer);
        }

        public void Send(byte id, string str) {
            this.kcp.Send(Encoding.UTF8.GetBytes(str));
        }

        public bool Connect() {
            if (this.Connected) {
                return false;
            }

            this.udp = new UdpClient(this.addr, this.port);
            this.kcp = new KCP(1, this.SendWrap);
            this.kcp.NoDelay(1, 10, 2, 1);
            this.kcp.WndSize(128, 128);
            this.Send(EventCode.Connect);
            this.Receive();
            this.updateTime = 0;
            this.Connected = true;

            return true;
        }

        public bool Disconnect(byte exitCode=ExitCode.Normal) {
            if (!this.Connected) {
                return false;
            }

            this.heartbeatTimer.Exit();
            this.Connected = false;
            this.udp.Close();

            var obj = new Datas.Disconnect() {
                exitCode = exitCode
            };
            this.SendEvent(EventCode.Disconnect, JsonUtility.ToJson(obj));

            return true;
        }

        public void RegisterEvent(byte id, Action<byte, string> Callback) {
            if (!this.eventHandler.ContainsKey(id)) {
                this.eventHandler.Add(id, new List<Action<byte, string>>());
            }

            this.eventHandler[id].Add(Callback);
        }

        private void SendEvent(byte id, string data) {
            if (this.eventHandler.ContainsKey(id)) {
                for (int i = 0; i < this.eventHandler[id].Count; i++) {
                    this.eventHandler[id][i](id, data);
                }
            }
        }

        private void Receive() {
            this.udp.BeginReceive(this.ReceiveCallback, null);
        }

        private Packet Recv(byte[] buffer) {
            var packet = new Packet() {
                id = buffer[0],
                data = Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1)
            };

            return packet;
        }

        private void ReceiveCallback(IAsyncResult ar) {
            try {
                var data = this.udp.EndReceive(ar, ref EP);

                if (data != null) {
                    this.heartbeat = true;
                    this.kcp.Input(data);
                }

                this.Receive();
            }
            catch (SocketException) {
                this.willDisconnect = true;
            }
        }

        private void SendCallback(IAsyncResult ar) {
            this.udp.EndSend(ar);
        }

        private void SendWrap(byte[] data, int size) {
            try {
                this.udp.BeginSend(data, size, this.SendCallback, null);
            }
            catch (SocketException) {
                this.Disconnect();
            }
        }

        private void HeartbeatTick() {
            if (!this.heartbeat) {
                this.Disconnect();
            }
            else {
                this.Send(EventCode.Heartbeat);
                this.heartbeat = false;
                this.heartbeatTimer.Enter(HEARTBEAT_INTERVAL, this.HeartbeatTick);
            }
        }
    }
}