using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
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

    void OnCreateInstance()
    {
        LevelPatrolPoints = GetComponentsInChildren<PatrolPoint>();
    }
}
