using UnityEngine;
using UnityEngine.UI;

namespace PPong.UI
{
    public class PongHUD : MonoBehaviour
    {
        [SerializeField]
        Text m_score;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            m_score.text = string.Format("{0}\n{1}", Game.PongGame.Instance.GetScore(Game.PongGame.Side.B), Game.PongGame.Instance.GetScore(Game.PongGame.Side.A));
        }
    }
}