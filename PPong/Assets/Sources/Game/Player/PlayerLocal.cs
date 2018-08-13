using System;
using UnityEngine;

namespace PPong.Game
{
    public class PlayerLocal : PlayerBase
    {
        public PlayerLocal(Racket racket) : base (racket)
        {
            
        }

        public override float GetNewRacketXPos()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        }
    }
}