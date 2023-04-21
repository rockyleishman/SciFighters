using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    internal static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ScoreManager>();
            }
            return _instance;
        }
    }

    internal int Score { get; private set; }

    private void Start()
    {
        Score = 0;
    }

    internal void AddScore(int value)
    {
        Score += value;
    }

    internal void SubtractScore(int value)
    {
        Score -= value;
    }
}
