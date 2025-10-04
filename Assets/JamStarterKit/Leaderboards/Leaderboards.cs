using System;
using System.Linq;
using GameBase.Utils;
using UnityEngine;

namespace GameBase.Leaderboards
{
    public class Leaderboards : MonoBehaviour
    {
        public ScoreRow RowPrefab;
        private int page;
        
        private void Start()
        {
            ScoreSystem.Instance().OnLoaded += ScoresLoaded;
            ScoreSystem.Instance().LoadLeaderboards(page);
        }

        private void ScoresLoaded()
        {
            var data = ScoreSystem.Instance().GetData();
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var go = transform.GetChild(i).gameObject;
                Destroy(go);
            }

            if (data == null)
            {
                return;
            }
            
            data.members.ToList().ForEach(entry =>
            {
                var row = Instantiate(RowPrefab, transform);
                row.Configure(entry.rank + ". " + entry.publicID, entry.score.AsScore());
            });
        }
        
        public void ChangePage(int direction)
        {
            if (page + direction < 0 || direction > 0 && ScoreSystem.Instance().EndReached) return;
            page = Mathf.Max(page + direction, 0);
            ScoreSystem.Instance().CancelLeaderboards();
            ScoreSystem.Instance().LoadLeaderboards(page);
        }
    }
}