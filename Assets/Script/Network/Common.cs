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
        public static byte Init = 1;
        public static byte PlayData = 2;
        public static byte Connect = 3;
        public static byte Report = 4;
        public static byte Comparison = 6;
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