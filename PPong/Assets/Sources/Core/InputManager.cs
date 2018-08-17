using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PPong.Core
{
    public class InputManager
    {
        float m_updateRate = 0.02f;

        float m_updateTimer = 0;

        public void ClientUpdate()
        {
            m_updateTimer += Time.deltaTime;
            if (m_updateTimer > m_updateRate)
            {
                PongNetworkManager.SendToServer(PongMsgType.PlayerInput, new InputMessage()
                {
                    MouseXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    TS = Time.time
                }, NetworkConfiguration.ChannelUnreliableSequenced);
                m_updateTimer = 0;
            }
        }
    }
}
