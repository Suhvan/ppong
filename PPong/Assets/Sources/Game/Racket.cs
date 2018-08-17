using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPong.Game
{
    public class Racket : MonoBehaviour
    {
        public Transform CachedTransform { get; private set; }

        private PongGame.Side m_currentSide;

        private BoxCollider2D m_collider;

        public PlayerBase Owner { get; set; }

        public float HalfSize { get; private set; }

        public PongGame.Side FieldSide
        {
            get
            {
                return m_currentSide;
            }
        }

        void Start()
        {            
            CachedTransform = transform;
            m_currentSide = PongGame.GetFieldSide(CachedTransform.position.y);
            m_collider = GetComponent<BoxCollider2D>();
            HalfSize = m_collider.size.x/2;
        }

        public void OnBallHit()
        {
            if (Owner is PlayerAI)
            {
                ((PlayerAI)Owner).OnBallHit();
            }
        }
    }
}
