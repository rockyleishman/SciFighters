using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] public float LifeTime = 0.05f;
    private LineRenderer _line;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }

    internal void Init(Vector3 start, Vector3 end, Color color)
    {
        _line.startColor = color;
        _line.endColor = color;
        _line.SetPosition(0, start);
        _line.SetPosition(1, end);

        Invoke("RemoveLaser", LifeTime);
    }

    private void RemoveLaser()
    {
        //////remove laser to pool
        Destroy(this.gameObject);
    }
}
