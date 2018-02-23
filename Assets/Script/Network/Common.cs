using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network {
    [Serializable]
    public struct SVector3 {
        public float x;
        public float y;
        public float z;

        public SVector3(Vector3 vector) {
            this.x = vector.x;
            this.y = vector.y;
            this.z = vector.z;
        }

        public Vector3 ToVector3() {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public struct InputData {
        public SVector3 mousePos;
        public bool isDown;
    }

    [Serializable]
    public class PlayData {
        public int[] connIds;
        public InputData[] inputDatas;
    }

    public static class EventCode {
        public const byte Connect = 0;
        public const byte Heartbeat = 1;
    }

    namespace Message {
        [Serializable]
        public class Init {
            public int seed;
            public int[] connIds;
        }

        [Serializable]
        public struct Comparison {
            public int playFrame;
            public string content;
        }
    }
}