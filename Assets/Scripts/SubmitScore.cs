using GameBase.Animations;
using GameBase.Effects;
using GameBase.Leaderboards;
using TMPro;
using UnityEngine;

namespace App
{
    public class SubmitScore : MonoBehaviour
    {
        public int Score;
        public Pulser ScoreTextPulser;
        public TMP_Text ScoreText;
        public Shaker ScoreTextShaker;

        public void IncrementScore(int amount)
        {
            Score += amount;
            ScoreTextPulser.Pulsate();
            ScoreTextShaker.Shake();
            ScoreText.text = $"{Score}";
            //EffectsSystem.AddEffect(0, Vector3.zero);
        }

        public void IncreaseScore()
        {
            IncrementScore(1);
        }
        
        public void SubmitFakeScore()
        {
            ScoreSystem.Instance().OnScoreLoaded += () =>
            {
                if (ScoreSystem.Instance().GetMemberData() == null || ScoreSystem.Instance().GetMemberData().score < Score)
                {
                    ScoreSystem.Instance().SaveScore(PlayerPrefs.GetString("PlayerName"), Score);
                }
            };
            ScoreSystem.Instance().LoadScore(PlayerPrefs.GetString("PlayerName"));
        }
    }
}