using UnityEngine;
using UnityEngine.Networking;

namespace PPong.Network
{
    public static class PongMsgType
    {
        public static readonly short CheckProto = 1001;
        public static readonly short Snapshot = 1002;
        public static readonly short PlayerInput = 1003;
        public static readonly short ResetBall = 1004;
    }


    public class SnapshotMessage : MessageBase
    {
        public Vector2 BallPos;
        public float RacketAXPos;
        public float RacketBXPos;
        public int ScoreA;
        public int ScoreB;
        public float TS;
    }

    public class InputMessage : MessageBase
    {
        public float MouseXPos;
        public float TS;
    }

    public class ResetBallMessage : MessageBase
    {
        public int BallIndx;
    }

}