using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network {
    [Serializable]
    public struct SVector {
        public float x;
        public float y;
        public float z;

        public SVector(Vector3 vector) {
            this.x = vector.x;
            this.y = vector.y;
            this.z = vector.z;
        }

        public Vector3 ToVector3() {
            return new Vector3(this.x, this.y, this.z);
        }
    }

    [Serializable]
    public struct InputData {
        public SVector mousePos;
        public bool isDown;
    }

    [Serializable]
    public class PlayData {
        public string[] fds;
        public InputData[] inputDatas;
    }

    public static class EventCode {
        public const byte Disconnect = 0;
        public const byte Connect = 1;
        public const byte Heartbeat = 2;
        public const byte Start = 3;
        public const byte Input = 4;
        public const byte Comparison = 5;
    }

    namespace Datas {
        [Serializable]
        public struct Connect {
            public string fd;
        }

        [Serializable]
        public struct Start {
            public int seed;
            public string leftFd;
            public string rightFd;
        }

        [Serializable]
        public struct Comparison {
            public int playFrame;
            public string content;
        }
    }
}