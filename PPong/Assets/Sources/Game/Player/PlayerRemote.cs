using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPong.Game
{
    public class PlayerRemote : PlayerBase
    {
        private float m_remoteMousePos = 0;

        public PlayerRemote(Racket racket) : base (racket)
        {

        }

        public override float GetNewRacketXPos()
        {
            return m_remoteMousePos;
        }

        public void OnNewMousePos(float pos)
        {
            m_remoteMousePos = pos;
        }
    }
}
