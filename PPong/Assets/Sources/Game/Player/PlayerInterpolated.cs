using PPong.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPong.Game
{
    public class PlayerInterpolated : PlayerBase
    {
        InterpolatingHelper InterpHelper { get; set; }

        public PlayerInterpolated(Racket racket) : base (racket)
        {
            InterpHelper = new InterpolatingHelper();
        }

        public override float GetNewRacketXPos()
        {
            return InterpHelper.GetInterpolatedPos().x;
        }

        public void OnNewSnapshot(float snapPos, float serverTime)
        {
            InterpHelper.OnNewSnapshot(new UnityEngine.Vector2(snapPos, 0), serverTime);
        }

    }
}
