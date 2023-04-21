using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWave : MonoBehaviour
{
    [SerializeField] public AIController[] EnemiesToSpawn;
    [SerializeField] public float HalfDelayFromLastSpawn;
    [SerializeField] public float HalfDelayToNextSpawn;
}
