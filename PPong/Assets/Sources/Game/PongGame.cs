using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPong.Game
{
    public class PongGame : MonoBehaviour
    {
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


        public Ball GameBall
        {
            get { return m_gameBall; }
        }

        private PlayerBase m_playerA;
        private PlayerBase m_playerB;

        public static PongGame Instance { private set; get; }


        void Awake()
        {
            GameMode = Mode.PlayerVsSelf;
            Instance = this;
            //m_playerA = new PlayerLocal(m_racketA);
            m_playerA = new PlayerAI(m_racketA, PlayerAI.Difficulty.Eazy);
            m_playerB = new PlayerAI(m_racketB, PlayerAI.Difficulty.Normal);
        }

        void Start()
        {

        }


        void FixedUpdate()
        {
            m_playerA.FixedPlayerUpdate();
            m_playerB.FixedPlayerUpdate();
        }

        public void OnBallScored()
        {

        }
    }
}
