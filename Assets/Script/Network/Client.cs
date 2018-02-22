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
        private static IPEndPoint EP;
        private const float UPDATE_INTERVAL = 0.02f;
        private const float HEARTBEAT_INTERVAL = 3;

        private UdpClient udp;
        private KCP kcp;
        private Timer updateTimer;
        private Timer heartbeatTimer;
        private bool heartbeat = true;

        public bool Connected {
            get;
            private set;
        }

        private void Receive() {
            this.udp.BeginReceive(this.ReceiveCallback, null);
        }

        private void Send(string data) {
            this.kcp.Send(Encoding.ASCII.GetBytes(data));
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
                this.Disconnect();
            }
        }

        private void SendCallback(IAsyncResult ar) {
            this.udp.EndSend(ar);
        }

        private void SendWrap(byte[] data, int size) {
            this.udp.BeginSend(data, size, this.SendCallback, null);
        }

        private void UpdateTick() {
            this.kcp.Update((uint)Mathf.FloorToInt(Time.time * 1000));
            this.updateTimer.Enter(UPDATE_INTERVAL, this.UpdateTick);
        }

        private void HeartbeatTick() {
            if (!this.heartbeat) {
                this.Disconnect();
            }
            else {
                this.Send("h");
                this.heartbeat = false;
                this.heartbeatTimer.Enter(HEARTBEAT_INTERVAL, this.HeartbeatTick);
            }
        }

        public Client(string addr, int port) {
            this.udp = new UdpClient(addr, port);
            this.updateTimer = new Timer();
            this.heartbeatTimer = new Timer();
        }

        public void Update() {
            if (!this.Connected) {
                return;
            }

            this.updateTimer.Update();
            this.heartbeatTimer.Update();

            for (var size = this.kcp.PeekSize(); size > 0; size = this.kcp.PeekSize()) {
                var buffer = new byte[size];

                if (this.kcp.Recv(buffer) > 0) {
                    string data = Encoding.ASCII.GetString(buffer);

                    if (data == "welcome") {
                        this.heartbeatTimer.Enter(HEARTBEAT_INTERVAL, this.HeartbeatTick);
                    }

                    Debug.Log(data);
                }
            }
        }

        public void Connect() {
            this.kcp = new KCP(1, this.SendWrap);
            //this.kcp.NoDelay(1, 10, 2, 1);
            //this.kcp.WndSize(128, 128);
            this.Send("connect");
            this.Receive();

            this.updateTimer.Enter(UPDATE_INTERVAL, this.UpdateTick);
            this.Connected = true;
            Debug.Log("connect");
        }

        public void Disconnect() {
            this.updateTimer.Exit();
            this.heartbeatTimer.Exit();
            this.Connected = false;
            Debug.Log("disconnect");
        }
    }
}