using TMPro;
using UnityEngine;

namespace GameBase.Leaderboards
{
    public class ScoreRow : MonoBehaviour
    {
        public TMP_Text Name;
        public TMP_Text Score;

        public void Configure(string n, string score)
        {
            Name.text = n;
            Score.text = score;
        }
    }
}