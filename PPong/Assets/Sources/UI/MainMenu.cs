using PPong.Core;
using PPong.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{	
	void Start ()
    {
        		
	}
	
	void Update ()
    {
		
	}

    public void StartPvSelf()
    {
        GameCore.Instance.PongSettings = new GameSettings() { GameMode = PPong.Game.PongGame.Mode.PlayerVsSelf };
        GameCore.Instance.ChangeGameState(GameCore.State.Pong);
    }

    public void StartPvEasyAI()
    {
        StartPvAI(PlayerAI.Difficulty.Eazy);
    }

    public void StartPvNormalAI()
    {
        StartPvAI(PlayerAI.Difficulty.Normal);
    }

    public void StartPvHardAI()
    {
        StartPvAI(PlayerAI.Difficulty.Unreal);
    }

    public void StartPvPHost()
    {
        GameCore.Instance.PongSettings = new GameSettings() { GameMode = PPong.Game.PongGame.Mode.PvPHost };
        GameCore.Instance.ChangeGameState(GameCore.State.Pong);
    }

    public void StartPvPClient()
    {
        GameCore.Instance.PongSettings = new GameSettings() { GameMode = PPong.Game.PongGame.Mode.PvPClient };
        GameCore.Instance.ChangeGameState(GameCore.State.Pong);
    }

    public void StartPvAI( PlayerAI.Difficulty dif )
    {
        GameCore.Instance.PongSettings = new GameSettings() { GameMode = PongGame.Mode.PlayerVsAI, AIDifficulty = dif };
        GameCore.Instance.ChangeGameState(GameCore.State.Pong);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
