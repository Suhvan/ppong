using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPong.Game
{
    public class Ball : MonoBehaviour
    {
        private Rigidbody2D m_cachedRigidbody;

        private Transform m_cachedTransform;

        public float XPos
        {
            get { return m_cachedTransform.position.x; }
        }

        public PongGame.Side CurrentFieldSide
        {
            get
            {
                return PongGame.GetFieldSide(m_cachedTransform.position.y);
            }
        }

        public PongGame.Side CurrentDirection
        {
            get
            {
                return PongGame.GetMovementDirection(m_cachedRigidbody.velocity.y);
            }
        }


       [SerializeField]
        private float m_ballSpeed = 10;

        public float BallSpeed { get { return m_ballSpeed; } }

        void Awake()
        {
            m_cachedRigidbody = gameObject.GetComponent<Rigidbody2D>();
            m_cachedTransform = transform;
        }

        void Start()
        {
            
        }

        public void Reset()
        {
            m_cachedTransform.position = Vector2.zero;
            m_cachedRigidbody.velocity = Vector2.zero;
        }

        public IEnumerator GiveInitialImpulse(PongGame.Side targetSide, float delay)
        {
            yield return new WaitForSeconds(delay);
            float forceX = Random.Range(-1f, 1f);
            float forceY = targetSide == PongGame.Side.B ? 1 : -1;
            SetVelocity(new Vector2(forceX, forceY));
        }

        void SetVelocity(Vector2 direction)
        {
            m_cachedRigidbody.velocity = direction.normalized * m_ballSpeed;
        }

        void Update()
        {

        }

        void OnCollisionEnter2D(Collision2D col)
        {
            Racket racket = col.gameObject.GetComponent<Racket>();
            if (racket != null)
            {
                float sumX = 0;
                for (int i = 0; i < col.contactCount; i++)
                {
                    sumX += col.GetContact(i).point.x;
                }

                float forceX = sumX / col.contactCount - racket.transform.position.x;
                float forceY = CurrentFieldSide == PongGame.Side.A ? 1 : -1;

                if (Mathf.Abs(m_cachedTransform.position.y) > Mathf.Abs(racket.transform.position.y))
                    forceY *= -1;
                racket.OnBallHit();
                SetVelocity(new Vector2(forceX, forceY));
            }

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.name == "border")
            {
                PongGame.Instance.OnBallScored( CurrentFieldSide );
            }
        }




    }
}