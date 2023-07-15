using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        playerScoreData.list.Add(new PlayerScore(ScoreManager.Instance.Score, ScoreManager.Instance.playerName));
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

    public bool HasNewHighScore => ScoreManager.Instance.Score > LoadPlayerScoreData().list[9].score;
    [SerializeField]private RectTransform highScoreLeaderboardContainer;
    public void UpdateHighScoreLeaderboard()
    {
        var playerScoreList = LoadPlayerScoreData().list;
        for (int i = 0; i < highScoreLeaderboardContainer.childCount; i++)
        {
            var child = highScoreLeaderboardContainer.GetChild(i);
            child.Find("Score").GetComponent<Text>().text = playerScoreList[i].score.ToString();
            child.Find("Name").GetComponent<Text>().text = playerScoreList[i].playerName;
            
        }
        
    }

    #endregion
}