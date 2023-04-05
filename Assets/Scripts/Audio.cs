using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    [SerializeField] public float LifeTime = 1.0f;

    void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.volume = source.volume / Vector3.Distance(transform.position, Camera.main.transform.position);
        
        Invoke("DestroyThis", LifeTime);
    }

    private void DestroyThis()
    {
        //////remove audio to pool
        Destroy(this.gameObject);
    }
}
