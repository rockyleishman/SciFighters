using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AISpawnManager : MonoBehaviour
{
    private static AISpawnManager _instance;

    public static AISpawnManager Instance
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

    [SerializeField] public AIController[] AIPrefabs;
    internal SpawnPoint[] SpawnPoints { get; private set; }
    [SerializeField] public List<SpawnWave> SpawnWaves;

    private void Start()
    {
        SpawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    private void Update()
    {
        foreach (SpawnWave wave in SpawnWaves.Reverse<SpawnWave>())
        {
            if (GameManager.Instance.GameTime >= wave.SpawnTime)
            {
                Transform spawnTransform = GameManager.Instance.Player.transform;

                //spawn far from player
                foreach (SpawnPoint point in SpawnPoints)
                {
                    if (Vector3.Distance(point.transform.position, GameManager.Instance.Player.transform.position) > Vector3.Distance(spawnTransform.position, GameManager.Instance.Player.transform.position))
                    {
                        spawnTransform = point.transform;
                    }
                }

                SpawnSpawnWave(wave, spawnTransform);
            }
        }
    }

    private void SpawnSpawnWave(SpawnWave wave, Transform spawnTransform)
    {
        foreach (int i in wave.AIIndicesToSpawn)
        {
            SpawnAI(AIPrefabs[i], spawnTransform);
        }

        SpawnWaves.Remove(wave);

        ////////
        Debug.Log("Wave Spawned");
        ////
    }

    private void SpawnAI(AIController aIPrefab, Transform spawnTransform)
    {
        Instantiate(aIPrefab, spawnTransform);
    }
}
