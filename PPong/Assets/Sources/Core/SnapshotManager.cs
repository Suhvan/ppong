﻿using PPong.Game;
using PPong.Network;
using UnityEngine;

namespace PPong.Core
{
    public class SnapshotManager
    {   
        float m_updateRate = 0.02f;
        float m_updateTimer = 0;

        public void Reset()
        {
            m_updateTimer = 0;
        }

        public SnapshotMessage CreatePongSnapshot()
        {
            return new SnapshotMessage()
            {
                BallPos = PongGame.Instance.GameBall !=null ? new Vector2( PongGame.Instance.GameBall.CachedTransform.position.x, PongGame.Instance.GameBall.CachedTransform.position.y) : Vector2.zero,
                RacketAXPos = PongGame.Instance.GetRacketPos(PongGame.Side.A),
                RacketBXPos = PongGame.Instance.GetRacketPos(PongGame.Side.B),
                ScoreA = PongGame.Instance.GetScore(PongGame.Side.A),
                ScoreB = PongGame.Instance.GetScore(PongGame.Side.B),
                TS = Time.time
            };
        }

        public void ServerUpdate()
        {
            m_updateTimer += Time.deltaTime;
            if (m_updateTimer > m_updateRate)
            {
                PongNetworkManager.SendToClients(PongMsgType.Snapshot, CreatePongSnapshot(), NetworkConfiguration.ChannelUnreliableSequenced);
                m_updateTimer = 0;
            }
        }
     
    }
}
