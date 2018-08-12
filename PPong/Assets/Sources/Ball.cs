using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    private Rigidbody2D m_cachedRigidbody;

    [SerializeField]
    private float m_ballSpeed = 8;
    

    void Awake()
    {
        m_cachedRigidbody = gameObject.GetComponent<Rigidbody2D>();
    }
        
    void Start () {
        GiveInitialImpulse();
    }

    void GiveInitialImpulse(bool up = true)
    {
        float forceX = Random.Range(-1f, 1f);
        float forceY = up ? 1 : -1;
        SetVelocity(new Vector2(forceX, forceY).normalized * m_ballSpeed);
    }

    void SetVelocity(Vector2 force)
    {   
        m_cachedRigidbody.velocity = force;
    }
	
	void Update () {
     
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

            float forceX = sumX / 2f - racket.transform.position.x;
            float forceY = transform.position.y > 0 ? -1 : 1;

            if (Mathf.Abs(transform.position.y) > Mathf.Abs(racket.transform.position.y))
                forceY *= -1;

            SetVelocity(new Vector2(forceX, forceY).normalized * m_ballSpeed);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.gameObject.name == "border")
        {
            bool topScored = transform.position.y < 0;            
            transform.position = Vector2.zero;
            GiveInitialImpulse(topScored);
        }
        else
        {   
            m_cachedRigidbody.velocity = new Vector2(-m_cachedRigidbody.velocity.x, m_cachedRigidbody.velocity.y);
        }
        
            
    }


    

}
