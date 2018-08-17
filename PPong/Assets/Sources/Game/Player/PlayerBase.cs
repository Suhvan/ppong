using UnityEngine;

namespace PPong.Game
{
    public abstract class PlayerBase
    {
        protected Racket m_racket;
        public int Score { get; set; }

        public PongGame.Side FieldSide { get { return m_racket.FieldSide; } }

        public PlayerBase(Racket racket)
        {
            m_racket = racket;
            m_racket.Owner = this;
            Score = 0;
        }

        public abstract float GetNewRacketXPos();
        
        public void FixedPlayerUpdate()
        {
            if (m_racket == null || m_racket.CachedTransform == null)
                return;

            if (PongGame.Instance.GameMode == PongGame.Mode.PvPClient && FieldSide == PongGame.Side.A)
                return;

            if (PongGame.Instance.GameMode == PongGame.Mode.PvPHost && FieldSide == PongGame.Side.B)
                return;


            float newRacketPos = Mathf.Clamp(GetNewRacketXPos(), PongGame.Instance.WestBorder + m_racket.HalfSize, PongGame.Instance.EastBorder - m_racket.HalfSize);
            m_racket.CachedTransform.position = new Vector2(newRacketPos, m_racket.CachedTransform.position.y);
        }
        
    }
}

