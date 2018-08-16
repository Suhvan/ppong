using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPong.Game
{
    public class PongGame : MonoBehaviour
    {

        private const float BALL_IMPULSE_DELAY = 0.5f;

        public enum Mode
        {
            PlayerVsSelf,
            PlayerVsAI
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

        private Mode GameMode { get; set; }

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

        public static PongGame Instance { private set; get; }


        void Awake()
        {
            GameMode = GameCore.Instance.PongSettings.GameMode;
            Instance = this;

            switch (GameMode)
            {
                case Mode.PlayerVsSelf:
                    m_playerA = new PlayerLocal(m_racketA);
                    m_playerB = new PlayerLocal(m_racketB);
                    break;
                case Mode.PlayerVsAI:
                    m_playerA = new PlayerLocal(m_racketA);
                    m_playerB = new PlayerAI(m_racketB, GameCore.Instance.PongSettings.AIDifficulty);
                    break;
            }

            EastBorder = m_eastWall.position.x;
            WestBorder = m_westWall.position.x;
        }

        void Start()
        {
            StartCoroutine(m_gameBall.GiveInitialImpulse(Side.B, 0));
        }

        void FixedUpdate()
        {
            m_playerA.FixedPlayerUpdate();
            m_playerB.FixedPlayerUpdate();
        }

        public int GetScore(Side side)
        {
            return GetPlayer(side).Score;
        }

        public void OnBallScored(Side ballSide)
        {
            Side winnerSide = ballSide == Side.A ? Side.B : Side.A;
            GetPlayer(winnerSide).Score++;
            m_gameBall.Reset();
            StartCoroutine(m_gameBall.GiveInitialImpulse(winnerSide, BALL_IMPULSE_DELAY));
        }

       
    }
}
