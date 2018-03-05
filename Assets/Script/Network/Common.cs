using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network {
    [Serializable]
    public struct InputData {
        public float movingValue;
        public bool willElaste;
    }

    [Serializable]
    public class PlayData {
        public string[] addrs;
        public InputData[] inputs;
    }

    public static class EventCode {
        public const byte Disconnect = 0;
        public const byte Connect = 1;
        public const byte Heartbeat = 2;
        public const byte Start = 3;
        public const byte Input = 4;
        public const byte Comparison = 5;
        public const byte Handshake = 6;
    }

    public static class ExitCode {
        public const byte None = 0;
        public const byte Normal = 1;
        public const byte Full = 2;
        public const byte Version = 3;
    }

    namespace Datas {
        [Serializable]
        public struct Connect {
            public string addr;
            public int version;
            public bool isFull;
        }

        [Serializable]
        public struct Disconnect {
            public byte exitCode;
        }

        [Serializable]
        public struct Start {
            public int seed;
            public string leftAddr;
            public string rightAddr;
        }

        [Serializable]
        public struct Input {
            public int frame;
            public InputData data;
        }

        [Serializable]
        public struct Comparison {
            public int playFrame;
            public string content;
        }

        [Serializable]
        public struct Handshake {
            public string deviceModel;
        }
    }
}