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

    internal SpawnPoint[] SpawnPoints { get; private set; }

    [SerializeField] public List<SpawnWave> SpawnWaves;
    [SerializeField] public SpawnWave DefaultSpawnWave;

    internal float SpawnTimer { get; private set; }

    private void Start()
    {
        SpawnPoints = GetComponentsInChildren<SpawnPoint>();

        SpawnTimer = SpawnWaves.DefaultIfEmpty(DefaultSpawnWave).First().HalfDelayFromLastSpawn;
    }

    private void Update()
    {
        SpawnTimer -= Time.deltaTime;

        if (SpawnTimer <= 0.0f)
        {
            Transform spawnTransform = GameManager.Instance.Player.transform;

            while (Vector3.Distance(spawnTransform.position, GameManager.Instance.Player.transform.position) < MinimumSpawnDistance)
            {
                spawnTransform = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform;
            }

            SpawnSpawnWave(SpawnWaves.DefaultIfEmpty(DefaultSpawnWave).First(), spawnTransform);
        }
    }

    private void SpawnSpawnWave(SpawnWave wave, Transform spawnTransform)
    {
        //add spawn delay from this wave for next spawn
        SpawnTimer = wave.HalfDelayToNextSpawn;

        //spawn enemies
        foreach (AIController enemy in wave.EnemiesToSpawn)
        {
            SpawnAI(enemy, spawnTransform);
        }

        //delete unique spawned waves
        try
        {
            SpawnWaves.RemoveAt(0);
        }
        catch
        {
            //no unique waves left
        }

        //add spawn delay from next wave for next spawn
        SpawnTimer += SpawnWaves.DefaultIfEmpty(DefaultSpawnWave).First().HalfDelayFromLastSpawn;
    }

    private void SpawnAI(AIController aIPrefab, Transform spawnTransform)
    {
        Instantiate(aIPrefab, spawnTransform);
    }
}
