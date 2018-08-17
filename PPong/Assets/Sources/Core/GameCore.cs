using PPong.Game;
using PPong.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PPong.Core
{
    public class GameCore : MonoBehaviour
    {
        private const string MENU_SCENE_NAME = "Menu";
        private const string PONG_SCENE_NAME = "Pong";

        public GameSettings PongSettings { get; set; }
        public SnapshotManager SnapshotManager { get; private set; }
        public InputManager InputManager { get; private set; }

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

            SnapshotManager = new SnapshotManager();
            InputManager = new InputManager();
            Instance = this;
            DontDestroyOnLoad(this);

            PongSettings = new GameSettings() { GameMode = PongGame.Mode.PvPHost };

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


        void Start()
        {

        }


        void Update()
        {

        }

        public void ChangeGameState(State targetState)
        {
            if (targetState == CurrentState)
                return;

            ShutdownOldState(CurrentState);
            PrepareNewState(targetState);

            CurrentState = targetState;
        }

        private void ShutdownOldState(State oldState)
        {
            switch (oldState)
            {
                case State.Pong:
                    {
                        if (PongGame.Instance == null)
                            return;
                        PongGame.Instance.Session.Shutdown();
                        return;
                    }
            }
        }

        private void PrepareNewState(State newState)
        {
            switch (newState)
            {
                case State.Pong:                   
                    InputManager.Reset();
                    SnapshotManager.Reset();
                    SceneManager.LoadScene(PONG_SCENE_NAME);
                    break;
                case State.Menu:
                    SceneManager.LoadScene(MENU_SCENE_NAME);
                    break;
            }
        }

    }
}
