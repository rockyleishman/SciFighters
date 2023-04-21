using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWave : MonoBehaviour
{
    [SerializeField] public int[] AIIndicesToSpawn;
    [SerializeField] public float DelayFromLastSpawn;
    internal bool isSpawned = false;
}
