using PPong.Core;
using PPong.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    GameObject m_buttonsPanel;

    [SerializeField]
    GameObject m_joinPanel;

    [SerializeField]
    InputField m_connectionAddress;

    void Start ()
    {
        m_buttonsPanel.SetActive(true);
        m_joinPanel.SetActive(false);
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

    public void StartJoin()
    {
        m_buttonsPanel.SetActive(false);
        m_joinPanel.SetActive(true);
        m_connectionAddress.text = PPong.Network.PongNetworkManager.DEFAULT_ADDRESS;
    }

    public void StartPvPClient()
    {
        GameCore.Instance.PongSettings = new GameSettings() { GameMode = PPong.Game.PongGame.Mode.PvPClient, ConnectionAddress = m_connectionAddress.text };
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
