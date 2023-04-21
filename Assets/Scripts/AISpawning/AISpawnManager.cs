using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AISpawnManager : MonoBehaviour
{
    private static AISpawnManager _instance;

    internal static AISpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AISpawnManager>();
            }
            return _instance;
        }
    }

    [SerializeField] public float MinimumSpawnDistance = 40.0f; //if this is too high for the level size the game will crash

    [SerializeField] public AIController[] AIPrefabs;

    internal SpawnPoint[] SpawnPoints { get; private set; }

    [SerializeField] public List<SpawnWave> SpawnWaves;
    [SerializeField] public SpawnWave DefaultSpawnWave;

    internal float SpawnTimer { get; private set; }

    private void Start()
    {
        SpawnPoints = GetComponentsInChildren<SpawnPoint>();

        SpawnTimer = SpawnWaves.DefaultIfEmpty(DefaultSpawnWave).First().DelayFromLastSpawn;
    }

    private void Update()
    {
        SpawnTimer -= Time.deltaTime;

        if (SpawnTimer <= 0.0f)
        {
            //////
            Debug.Log("spawning spawnwave " + SpawnWaves.DefaultIfEmpty(DefaultSpawnWave).First().name);
            //////
            Transform spawnTransform = GameManager.Instance.Player.transform;

            while (Vector3.Distance(spawnTransform.position, GameManager.Instance.Player.transform.position) < MinimumSpawnDistance)
            {
                spawnTransform = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform;
            }

            SpawnSpawnWave(SpawnWaves.DefaultIfEmpty(DefaultSpawnWave).First(), spawnTransform);
        }

        /*foreach (SpawnWave wave in SpawnWaves)
        {
            if (GameManager.Instance.GameTime >= wave.SpawnTime)
            {
                Transform spawnTransform = GameManager.Instance.Player.transform;

                while (Vector3.Distance(spawnTransform.position, GameManager.Instance.Player.transform.position) < MinimumSpawnDistance)
                {
                    spawnTransform = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform;
                }
                
                SpawnSpawnWave(wave, spawnTransform);
            }
        }

        foreach (SpawnWave wave in SpawnWaves.Reverse<SpawnWave>())
        {
            if (wave.isSpawned)
            {
                SpawnWaves.Remove(wave);
            }
        }*/
    }

    private void SpawnSpawnWave(SpawnWave wave, Transform spawnTransform)
    {
        foreach (int i in wave.AIIndicesToSpawn)
        {
            SpawnAI(AIPrefabs[i], spawnTransform);
        }

        //wave.isSpawned = true;
        try
        {
            SpawnWaves.RemoveAt(0);
        }
        catch
        {
            //no unique SpawnWaves left
        }

        //reset spawn timer for next wave
        SpawnTimer = SpawnWaves.DefaultIfEmpty(DefaultSpawnWave).First().DelayFromLastSpawn;
    }

    private void SpawnAI(AIController aIPrefab, Transform spawnTransform)
    {
        Instantiate(aIPrefab, spawnTransform);
    }
}
