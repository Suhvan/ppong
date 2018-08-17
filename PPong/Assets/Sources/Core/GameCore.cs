using PPong.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCore : MonoBehaviour
{
    private const string MENU_SCENE_NAME = "Menu";
    private const string PONG_SCENE_NAME = "Pong";

    public GameSettings PongSettings { get; set; }

    public enum State
    {
        Menu,
        Pong
    }

    public State CurrentState { get; private set; }

    public static GameCore Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        PongSettings = new GameSettings() { GameMode = PongGame.Mode.PvPHost};

        switch (SceneManager.GetActiveScene().name)
        {
            case MENU_SCENE_NAME:
                CurrentState = State.Menu;
                break;
            case PONG_SCENE_NAME:
                CurrentState = State.Pong;
                break;
        }
        
    }

    
    void Start ()
    {
		
	}
	
	
	void Update ()
    {
		
	}

    public void ChangeGameState(State targetState)
    {
        if (targetState == CurrentState)
            return;
        switch (targetState)
        {
            case State.Pong:
                SceneManager.LoadScene(PONG_SCENE_NAME);
                break;
            case State.Menu:
                SceneManager.LoadScene(MENU_SCENE_NAME);
                break;
        }
        CurrentState = targetState;
    }


    
}
