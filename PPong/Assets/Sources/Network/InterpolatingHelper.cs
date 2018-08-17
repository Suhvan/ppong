using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PPong.Network
{

    public struct PosSnapshot
    {
        public Vector2 Position;
        public float Ts;
    }

    public class InterpolatingHelper
    {
        private float m_serverTime = -1;
        private float m_syncTime = -1;
        private float m_clientTime
        {
            get
            {
                return m_serverTime + Time.time - m_syncTime;
            }
        }

        private static float s_interpDelay = 0.1f;
        private static float s_sqrSnapDistance = 9;

        public bool SnappingEnabled { get; set; }
        private List<PosSnapshot> m_snapshots = new List<PosSnapshot>();


        public void OnNewSnapshot(Vector2 snapPos, float serverTime)
        {            
            m_syncTime = Time.time;
            m_serverTime = serverTime;

            if (SnappingEnabled && m_snapshots.Count > 1)
            {
                if ((snapPos - m_snapshots[m_snapshots.Count - 1].Position).sqrMagnitude > s_sqrSnapDistance)
                {
                    m_snapshots.Clear();
                }
            }
            
            m_snapshots.Add(new PosSnapshot()
            {
                Position = snapPos,
                Ts = serverTime
            });
        }

        public Vector2 GetInterpolatedPos()
        {
            var renderTime = m_clientTime - s_interpDelay;

            while (m_snapshots.Count > 2)
            {
                if (m_snapshots[1].Ts >= renderTime)
                    break;

                m_snapshots.RemoveAt(0);
            }

            Vector2 newPos = Vector2.zero;
            if (m_snapshots.Count > 1)
            {  

                int idx = m_snapshots.Count - 1;
                for (int i = 1; i < m_snapshots.Count; i++)
                {
                    if (m_snapshots[i].Ts >= renderTime)
                    {
                        idx = i;
                        break;
                    }
                }

                var s1 = m_snapshots[idx - 1];
                var s2 = m_snapshots[idx];

                var t = (renderTime - s1.Ts) / (s2.Ts - s1.Ts);

                newPos = s1.Position + (s2.Position - s1.Position) * t;
            }
            else if (m_snapshots.Count == 1)
            {
                newPos = m_snapshots[0].Position;
            }

            return newPos;

        }
    }

   
}
