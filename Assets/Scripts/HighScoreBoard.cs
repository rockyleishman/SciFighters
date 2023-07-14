using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreBoard : MonoBehaviour
{
    #region High Score System

    [System.Serializable]
    public class PlayerScore
    {
        public int score;
        public string playerName;

        public PlayerScore(int score, string playerName)
        {
            this.score = score;
            this.playerName = playerName;
        }
    }

    [System.Serializable]
    public class PlayerScoreData
    {
        public List<PlayerScore> list = new List<PlayerScore>();
    }

    private readonly string SaveFileName = "player_score_json";
    private string playerName = "No Name";

    public void SavePlayerScoreData()
    {
        var playerScoreData = LoadPlayerScoreData();
        playerScoreData.list.Add(new PlayerScore(ScoreManager.Instance.Score, playerName));
        // arrange the score
        playerScoreData.list.Sort((x, y) => y.score.CompareTo(x.score));
        SaveSystem.Save(SaveFileName, playerScoreData);
    }

    PlayerScoreData LoadPlayerScoreData()
    {
        var playerScoreData = new PlayerScoreData();
//检查分数列表文件是否为空
        if (SaveSystem.SaveFileExists(SaveFileName))
        {
            playerScoreData = SaveSystem.Load<PlayerScoreData>(SaveFileName);
        }
        else
        {
            while (playerScoreData.list.Count < 10)
            {
                playerScoreData.list.Add(new PlayerScore(0, playerName));
            }

            SaveSystem.Save(SaveFileName, playerScoreData);
        }

        return playerScoreData;
    }

    #endregion
}