using UnityEngine;

namespace PPong.Game
{
    public class PlayerAI : PlayerBase
    {
        public enum Difficulty
        {
            Eazy, Normal, Unreal
        }

        private float m_racketMaxSpeed;
        private float m_curSpeed;
        private float m_accel;

        private Difficulty m_dif;

        private float m_racketShift = 0;
        

        public PlayerAI(Racket racket, Difficulty dif) : base(racket)
        {
            m_dif = dif;
            m_racketMaxSpeed = dif == Difficulty.Unreal? 9 : 4.5f;
            m_accel = dif == Difficulty.Unreal ? 30f : 15f;
            m_curSpeed = 0;
            GenereateNewShift();
        }

        public override float GetNewRacketXPos()
        {
            float racketPos = m_racket.CachedTransform.position.x,
                targetPos = PongGame.Instance.GameBall.XPos + m_racketShift;

            if (m_dif == Difficulty.Eazy && FieldSide != PongGame.Instance.GameBall.CurrentDirection)
            {
                SlowDown();
            }
            else
            {
                AdjustSpeed(Mathf.Abs(racketPos - targetPos) / Time.deltaTime);
            }

            return Mathf.MoveTowards(racketPos, targetPos, m_curSpeed * Time.deltaTime);

        }

        private void SlowDown()
        {
            m_curSpeed = Mathf.MoveTowards(m_curSpeed, 0, m_accel * Time.deltaTime);
        }

        private void AdjustSpeed(float maxNeededSpeed)
        {
            if (m_curSpeed > maxNeededSpeed)
            {
                m_curSpeed = maxNeededSpeed;
                return;
            }
            m_curSpeed = Mathf.MoveTowards(m_curSpeed, m_racketMaxSpeed, m_accel * Time.deltaTime);
        }

        public void OnBallHit()
        {
            GenereateNewShift();
        }

        void GenereateNewShift()
        {
            m_racketShift = Random.Range(-1f, 1f);
        }
    }
}
