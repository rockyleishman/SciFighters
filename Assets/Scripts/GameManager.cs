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

    internal float GameTime { get; private set; }

    [SerializeField] public PlayerController Player;

    private void OnCreateInstance()
    {
        //get array of patrol points for AI navigation
        LevelPatrolPoints = GetComponentsInChildren<PatrolPoint>();

        //init game time
        GameTime = 0.0f;

        //init UI
        UIManager.Instance.UpdateUI();
    }

    private void Update()
    {
        GameTime += Time.deltaTime;
    }

    internal void ResetGameTime()
    {
        GameTime = 0.0f;
    }
}
