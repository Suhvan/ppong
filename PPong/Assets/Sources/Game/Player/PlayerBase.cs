using UnityEngine;

namespace PPong.Game
{
    public abstract class PlayerBase
    {
        protected Racket m_racket;
        protected int m_score;

        public PongGame.Side FieldSide { get { return m_racket.FieldSide; } }

        public PlayerBase(Racket racket)
        {
            m_racket = racket;
            m_racket.Owner = this;
            m_score = 0;
        }

        public abstract float GetNewRacketXPos();
        
        public void FixedPlayerUpdate()
        {
            m_racket.CachedTransform.position = new Vector2(GetNewRacketXPos(), m_racket.CachedTransform.position.y);
        }
        
    }
}

