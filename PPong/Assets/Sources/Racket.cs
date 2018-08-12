using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour {
    Transform m_cachedTransform;
	// Use this for initialization
	void Start () {
        m_cachedTransform = transform;
    }
	
	// Update is called once per frame
	void Update () {
        
        m_cachedTransform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, m_cachedTransform.position.y);
	}
}
