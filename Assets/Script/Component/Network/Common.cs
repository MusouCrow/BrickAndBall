using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Component.Network {
    public class PlayData {
        public int[] newPlayers;
        public int frame;
        public int[] connIds;
        public InputData[] inputDatas;
    }

    public struct InputData {
        public Vector2 mousePos;
        public bool isDown;
    }

    public static class CustomMsgType {
        public static short AddPlayer = MsgType.Highest + 1;
        public static short AddPlayData = MsgType.Highest + 2;
        public static short AddConnection = MsgType.Highest + 3;
        public static short ReceiveReport = MsgType.Highest + 4;
        public static short DelConnection = MsgType.Highest + 5;
    }

    namespace Message {
        public class Empty : MessageBase {}

        public class AddPlayer : MessageBase {
            public int seed;
        }

        public class AddPlayData : MessageBase {
            public PlayData playData;
        }

        public class ReceiveReport : MessageBase {
            public int playFrame;
            public InputData inputData;
            public string comparison;
        }
    }
}


