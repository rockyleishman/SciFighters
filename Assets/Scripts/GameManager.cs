using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    internal static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                _instance.OnCreateInstance();
            }
            return _instance;
        }
    }

    internal PatrolPoint[] LevelPatrolPoints { get; private set; }

    [SerializeField] public float LevelTimeLimitInMinutes = 20;
    internal float GameTime { get; private set; }

    [SerializeField] public PlayerController Player;

    private void OnCreateInstance()
    {
        //init UI
        UIManager.Instance.UpdateUI();
    }

    private void Start()
    {
        //get array of patrol points for AI navigation
        LevelPatrolPoints = GetComponentsInChildren<PatrolPoint>();

        //init game time
        GameTime = 0.0f;
    }

    private void Update()
    {
        //update time
        GameTime += Time.deltaTime;
        UIManager.Instance.UpdateTime();

        //check for time up
        if (GameTime >= LevelTimeLimitInMinutes * 60.0f)
        {
            GameOver();
        }
    }

    internal void GameOver()
    {
        //game over
        UIManager.Instance.ShowEndGameMenu();
    }

    internal void ResetGameTime()
    {
        GameTime = 0.0f;
    }

    internal void Nothing()
    {
        //used to create an instance without doing anything else
    }
}
