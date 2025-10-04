using System;
using GameBase.SceneChanger;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App
{
    public class GameTimer: MonoBehaviour
    {
        public float InitialMatchDurationInSeconds = 60 * 3;
        public TMP_Text TimeText;
        public GameObject EndScreenWrapper;
        public TMP_Text WinnerText;
        private float MatchDurationInSeconds;
        private bool roundFinished = false;
        private LevelBuilder levelBuilder;
        private SubmitScore submitScore;
        private EnemyScore enemyScore;

        private void Start()
        {
            levelBuilder = FindFirstObjectByType<LevelBuilder>();
            enemyScore = FindFirstObjectByType<EnemyScore>();
            submitScore = FindFirstObjectByType<SubmitScore>();
            MatchDurationInSeconds = InitialMatchDurationInSeconds;
        }

        private void Update()
        {
            if (roundFinished)
            {
                return;
            }
            
            MatchDurationInSeconds -= Time.deltaTime;
            if (MatchDurationInSeconds <= 0)
            {
                MatchDurationInSeconds = 0;
                roundFinished = true;
                levelBuilder.DisablePlayerInput();
                ShowEndScreen();
            }
            var time = TimeSpan.FromSeconds(MatchDurationInSeconds).ToString(@"mm\:ss");
            TimeText.text = $"{time}";
        }

        private void ShowEndScreen()
        {
            EndScreenWrapper.SetActive(true);
            if (enemyScore.Score > submitScore.Score)
            {
                WinnerText.text = "You lose!";
            }
            else
            {
                WinnerText.text = "You win!";
            }
        }

        public void Restart()
        {
            submitScore.SubmitFakeScore();
            SceneChanger.Instance.ChangeScene("MainMenu");
        }
    }
}