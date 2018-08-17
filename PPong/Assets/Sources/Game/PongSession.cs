using PPong.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PPong.Game
{
    public class PongSession
    {
        public string PromptMessage { get; private set; }
        public bool Ready { get; private set; }

        private NetworkConnection EnemyConn { get; set; }

        private string m_address = PongNetworkManager.DEFAULT_ADDRESS;

        public void Init()
        {
            Ready = false;

            if (!string.IsNullOrEmpty(Core.GameCore.Instance.PongSettings.ConnectionAddress))
            {
                m_address = Core.GameCore.Instance.PongSettings.ConnectionAddress;
            }

            switch (PongGame.Instance.GameMode)
            {
                case PongGame.Mode.PlayerVsAI:
                case PongGame.Mode.PlayerVsSelf:
                    PromptMessage = "";
                    Ready = true;
                    return;
                case PongGame.Mode.PvPClient:
                    StartClient();
                    PromptMessage = string.Format("Connecting to {0}...", m_address);
                    return;
                case PongGame.Mode.PvPHost:
                    PromptMessage = "Awaiting enemy";
                    StartServer();
                    return;
            }
        }

        public void Shutdown()
        {          
            if (PongGame.Instance.IsClient)
                PongNetworkManager.CL_Shutdown();
            else if (PongGame.Instance.IsHost)
                PongNetworkManager.SV_Shutdown();

            return;
        }

        public void StartServer()
        {
            if (PongNetworkManager.StartServer(SV_OnDisconnected) < 0)
            {
                PromptMessage = "Failed to start server. Press Esc and try to host game again";
                return;
            }

            NetworkServer.RegisterHandler(PongMsgType.CheckProto, m =>
            {   
                if (EnemyConn != null && m.conn!= EnemyConn)
                {
                    //There can be only ONE
                    m.conn.Disconnect();
                    return;
                }
                EnemyConn = m.conn;
                PromptMessage = "";
                Ready = true;
            });

            NetworkServer.RegisterHandler(PongMsgType.PlayerInput, m =>
            {
                var msg = m.ReadMessage<InputMessage>();
                PongGame.Instance.ApplyPlayerInput(msg);
            });
        }

        private void SV_OnDisconnected(NetworkConnection conn)
        {
            if (conn == EnemyConn)
            {
                EnemyConn = null;
                PromptMessage = "Lost connection with enemy. Waiting for another one";
                Ready = false;
            }
        }

        public void StartClient()
        {
            var address = PongNetworkManager.DEFAULT_ADDRESS;
            if (!string.IsNullOrEmpty(Core.GameCore.Instance.PongSettings.ConnectionAddress))
            {
                address = Core.GameCore.Instance.PongSettings.ConnectionAddress;
            }
            var client = PongNetworkManager.ConnectClient(address, PongNetworkManager.PORT, CL_OnConnected, CL_OnDisconnected);

            client.RegisterHandler(PongMsgType.Snapshot, m =>
            {   
                var msg = m.ReadMessage<SnapshotMessage>();
                PongGame.Instance.ApplySnapshot(msg);                
            });

            client.RegisterHandler(PongMsgType.ResetBall, m =>
            {
                var msg = m.ReadMessage<ResetBallMessage>();
                PongGame.Instance.OnResetBall(msg);
            });
        }

        private void CL_OnConnected(NetworkConnection conn)
        {
            PongNetworkManager.SendToServer(PongMsgType.CheckProto, null, NetworkConfiguration.ChannelReliable);
            PromptMessage = "";
            Ready = true;
        }

        private void CL_OnDisconnected()
        {
            PromptMessage = string.Format("We got disconnected from server ({0}). Press Esc and try to join game again", m_address);
        }
    }
}
