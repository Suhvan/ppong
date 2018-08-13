using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPong.Game
{
    public class Racket : MonoBehaviour
    {
        public Transform CachedTransform { get; private set; }

        private PongGame.Side m_currentSide;

        public PlayerBase Owner { get; set; }

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
        }

        public void OnBallHit()
        {
            if (Owner is PlayerAI)
            {
                ((PlayerAI)Owner).OnBallHit();
            }
        }

        void Update()
        {
            //CachedTransform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, CachedTransform.position.y);
        }
    }
}
