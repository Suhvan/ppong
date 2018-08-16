using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour {

    public static GameCore Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);

        Instance = this;
        DontDestroyOnLoad(this);
    }

    
    void Start ()
    {
		
	}
	
	
	void Update ()
    {
		
	}

    
}
