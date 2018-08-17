using PPong.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PPong.Game
{
    public class PongGame : MonoBehaviour
    {
        private const float BALL_IMPULSE_DELAY = 0.5f;

        public enum Mode
        {
            PlayerVsSelf,
            PlayerVsAI,
            PvPClient,
            PvPHost
        }

        public enum Side
        {
            A = 0,
            B = 1
        }

        public static Side GetFieldSide(float yPos)
        {
            if (yPos > 0)
                return Side.B;
            return PongGame.Side.A;
        }

        public static Side GetMovementDirection(float yVelocity)
        {
            if (yVelocity > 0)
                return PongGame.Side.B;
            return PongGame.Side.A;
        }

        public Mode GameMode { get; private set; }

        public bool IsClient { get { return GameMode == Mode.PvPClient; }  }

        [SerializeField]
        private Ball m_gameBall;

        [SerializeField]
        private Racket m_racketA;

        [SerializeField]
        private Racket m_racketB;

        [SerializeField]
        private Transform m_eastWall;

        [SerializeField]
        private Transform m_westWall;

        [SerializeField]
        private List<Ball> m_ballPrefabs;
             

        public float EastBorder { get; private set; }
        public float WestBorder { get; private set; }


        public Ball GameBall
        {
            get { return m_gameBall; }
        }

        private PlayerBase m_playerA;
        private PlayerBase m_playerB;

        PlayerBase GetPlayer(Side side)
        {
            return side == Side.A ? m_playerA : m_playerB;
        }


        public float GetRacketPos(Side side)
        {
            switch (side)
            {
                case Side.A:
                    return m_racketA.CachedTransform.position.x;
                case Side.B:
                    return m_racketB.CachedTransform.position.x;
                default:
                    throw new System.NotSupportedException();
            }
        }

        public static PongGame Instance { private set; get; }


        void Awake()
        {
            Instance = this;

            EastBorder = m_eastWall.position.x;
            WestBorder = m_westWall.position.x;
        }

        void Start()
        {
            GameMode = GameCore.Instance.PongSettings.GameMode;

            switch (GameMode)
            {
                case Mode.PvPHost:
                    //m_playerA = new PlayerAI(m_racketA, GameCore.Instance.PongSettings.AIDifficulty);
                    m_playerA = new PlayerLocal(m_racketA);
                    m_playerB = new PlayerRemote(m_racketB);
                    break;
                case Mode.PvPClient:
                    m_playerA = new PlayerInterpolated(m_racketA);
                    m_playerB = new PlayerInterpolated(m_racketB);
                    break;
                case Mode.PlayerVsSelf:
                    m_playerA = new PlayerLocal(m_racketA);
                    m_playerB = new PlayerLocal(m_racketB);
                    break;
                case Mode.PlayerVsAI:
                    m_playerA = new PlayerLocal(m_racketA);
                    m_playerB = new PlayerAI(m_racketB, GameCore.Instance.PongSettings.AIDifficulty);
                    break;
            }

            

            //TODO move network related stuff to other class
            if (GameMode == Mode.PvPHost)
            {
                PongNetworkManager.StartServer(null);
            }

            if (GameMode == Mode.PvPClient)
            {
                PongNetworkManager.ConnectClient("localhost", PongNetworkManager.PORT, null, null);
            }

            //if (GameMode != Mode.PvPClient)
           //     CreateRandomBall();


            if (PongGame.Instance.IsClient)
                return;

            StartCoroutine(m_gameBall.GiveInitialImpulse(Side.B, 0));
        }        

        void FixedUpdate()
        {
            m_playerA.FixedPlayerUpdate();
            m_playerB.FixedPlayerUpdate();
        }

        void Update()
        {
            switch (GameMode)
            {
                case Mode.PvPHost:
                    GameCore.Instance.SnapshotManager.ServerUpdate();
                    break;
                case Mode.PvPClient:
                    GameCore.Instance.InputManager.ClientUpdate();
                    break;
            }
        }

        public int GetScore(Side side)
        {
            return GetPlayer(side).Score;
        }

        public void OnBallScored(Side ballSide)
        {
            if (PongGame.Instance.IsClient)
                return;
            Side winnerSide = ballSide == Side.A ? Side.B : Side.A;
            GetPlayer(winnerSide).Score++;
            GameBall.Reset();
           // DestroyOldBall();
           // CreateRandomBall();
            StartCoroutine(m_gameBall.GiveInitialImpulse(winnerSide, BALL_IMPULSE_DELAY));
        }

       /* private void CreateRandomBall()
        {
            if (GameMode != Mode.PvPClient)
                m_gameBall = Instantiate(m_ballPrefabs[Random.Range(0, m_ballPrefabs.Count)]);

            if (GameMode == Mode.PvPHost)   
            {   
                NetworkServer.Spawn(m_gameBall.gameObject);                
            }
        }


        private void DestroyOldBall()
        {
            if (GameMode == Mode.PvPClient)
                return;
            if (GameMode == Mode.PvPHost)
                NetworkServer.Destroy(m_gameBall.gameObject);
            else
                Destroy(m_gameBall.gameObject);
        }*/

        //TODO move it to snapshot manager

        public void ApplySnapshot(SnapshotMessage msg )
        {
            //m_gameBall.transform.position = msg.BallPos;
            m_gameBall.OnNewSnapshot(msg.BallPos, msg.TS);
            //TODO put this into Player
            //m_racketA.CachedTransform.position = new Vector2(msg.RacketAXPos, m_racketA.CachedTransform.position.y);
            (m_playerA as PlayerInterpolated).OnNewSnapshot(msg.RacketAXPos, msg.TS);
            (m_playerB as PlayerInterpolated).OnNewSnapshot(msg.RacketBXPos, msg.TS);
            m_playerA.Score = msg.ScoreA;
            m_playerB.Score = msg.ScoreB;
            
        }

        public void ApplyPlayerInput(InputMessage msg)
        {   
            //TODO put this into Player
            m_racketB.CachedTransform.position = new Vector2(msg.MouseXPos, m_racketB.CachedTransform.position.y);
            (m_playerB as PlayerRemote).OnNewMousePos(msg.MouseXPos);
        }

    }
}
