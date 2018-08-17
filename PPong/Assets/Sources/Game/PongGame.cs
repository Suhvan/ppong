using PPong.Core;
using PPong.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPong.Game
{
    public class PongGame : MonoBehaviour
    {
        private const float BALL_IMPULSE_DELAY = 0.5f;

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
        public bool IsHost { get { return GameMode == Mode.PvPHost; } }

        public PongSession Session { get; private set; }

        public float EastBorder { get; private set; }
        public float WestBorder { get; private set; }

        private Ball m_gameBall;
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
            Session = new PongSession();

            EastBorder = m_eastWall.position.x;
            WestBorder = m_westWall.position.x;
        }

        void Start()
        {
            GameMode = GameCore.Instance.PongSettings.GameMode;

            Session.Init();

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

            if (IsClient)
                return;

            StartCoroutine(ThrowBallWhenReady(Side.A));
        }

        IEnumerator ThrowBallWhenReady(Side targetSide)
        {
            while (!Session.Ready)
                yield return null;

            LaunchBall(targetSide);
        }
        

        void FixedUpdate()
        {
            if (!Session.Ready)
                return;

            m_playerA.FixedPlayerUpdate();
            m_playerB.FixedPlayerUpdate();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                GameCore.Instance.ChangeGameState(GameCore.State.Menu);
            }

            if (!Session.Ready)
                return;

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
            if (IsClient)
                return;

            Side winnerSide = ballSide == Side.A ? Side.B : Side.A;
            GetPlayer(winnerSide).Score++;
            DestroyOldBall();
            StartCoroutine(ThrowBallWhenReady(winnerSide));
        }

        private void LaunchBall(Side targetSide)
        {
            CreateRandomBall();
            GameBall.Reset();
            StartCoroutine(m_gameBall.GiveInitialImpulse(targetSide, BALL_IMPULSE_DELAY));
        }

       private void CreateRandomBall()
        {
            var ballIndex = Random.Range(0, m_ballPrefabs.Count);
            m_gameBall = Instantiate(m_ballPrefabs[ballIndex]);

            if (IsHost)   
            {
                //TODO move it some place else
                PongNetworkManager.SendToClients(PongMsgType.ResetBall, new ResetBallMessage() { BallIndx = ballIndex }, NetworkConfiguration.ChannelReliableSequenced);
            }
        }


        private void DestroyOldBall()
        {
            if (IsClient)
                return;
            
            Destroy(m_gameBall.gameObject);
        }

        public void ApplySnapshot(SnapshotMessage msg )
        {
            if (!IsClient)
                return;
            if(m_gameBall!=null)
                m_gameBall.OnNewSnapshot(msg.BallPos, msg.TS);
            (m_playerA as PlayerInterpolated).OnNewSnapshot(msg.RacketAXPos, msg.TS);
            (m_playerB as PlayerInterpolated).OnNewSnapshot(msg.RacketBXPos, msg.TS);
            m_playerA.Score = msg.ScoreA;
            m_playerB.Score = msg.ScoreB;
            
        }

        public void ApplyPlayerInput(InputMessage msg)
        {
            if (!IsHost)
                return;
            (m_playerB as PlayerRemote).OnNewMousePos(msg.MouseXPos);
        }

        public void OnResetBall(ResetBallMessage msg)
        {
            if (!IsClient)
                return;
            if(m_gameBall!=null)
                Destroy(m_gameBall.gameObject);
            m_gameBall = Instantiate(m_ballPrefabs[msg.BallIndx]);
        }

    }
}
