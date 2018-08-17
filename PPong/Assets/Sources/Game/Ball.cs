using PPong.Network;
using System.Collections;
using UnityEngine;

namespace PPong.Game
{
    public class Ball : MonoBehaviour
    {
        private Rigidbody2D m_cachedRigidbody;

        [SerializeField]
        private float m_ballSpeed = 10;

        public float BallSpeed { get { return m_ballSpeed; } }

        InterpolatingHelper m_interpHelper;
        InterpolatingHelper InterpHelper
        {
            get
            {
                if (m_interpHelper == null)
                    m_interpHelper = new InterpolatingHelper() { SnappingEnabled = true };
                return m_interpHelper;
            }
        }

        public Transform CachedTransform { get; private set; }

        public float XPos
        {
            get { return CachedTransform.position.x; }
        }

        public PongGame.Side CurrentFieldSide
        {
            get
            {
                return PongGame.GetFieldSide(CachedTransform.position.y);
            }
        }

        public PongGame.Side CurrentDirection
        {
            get
            {
                return PongGame.GetMovementDirection(m_cachedRigidbody.velocity.y);
            }
        }

        void Awake()
        {   
            m_cachedRigidbody = gameObject.GetComponent<Rigidbody2D>();
            
            if (PongGame.Instance.IsClient)
            {
                Destroy(m_cachedRigidbody);                
            }

            CachedTransform = transform;
           
        }

        void Start()
        {  
        }

        public void Reset()
        {
            if (PongGame.Instance.IsClient)
                return;
            CachedTransform.position = Vector2.zero;
            m_cachedRigidbody.velocity = Vector2.zero;
        }

        public IEnumerator GiveInitialImpulse(PongGame.Side targetSide, float delay)
        {
            if (PongGame.Instance.IsClient)
                yield break; 

            yield return new WaitForSeconds(delay);
            float forceX = Random.Range(-1f, 1f);
            float forceY = targetSide == PongGame.Side.B ? 1 : -1;
            SetVelocity(new Vector2(forceX, forceY));
        }

        void SetVelocity(Vector2 direction)
        {
            m_cachedRigidbody.velocity = direction.normalized * m_ballSpeed;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (PongGame.Instance.IsClient)
                return;

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

                if (Mathf.Abs(CachedTransform.position.y) > Mathf.Abs(racket.transform.position.y))
                    forceY *= -1;
                racket.OnBallHit();
                SetVelocity(new Vector2(forceX, forceY));
            }

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (PongGame.Instance.IsClient)
                return;

            PongGame.Instance.OnBallScored( CurrentFieldSide );
        }

        void Update()
        {
            if (!PongGame.Instance.IsClient)
                return;
            
            CachedTransform.position = InterpHelper.GetInterpolatedPos();
        }

        public void OnNewSnapshot(Vector2 snapPos, float serverTime)
        {
            InterpHelper.OnNewSnapshot(snapPos, serverTime);
        }

    }
}