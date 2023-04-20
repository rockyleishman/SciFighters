using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] public float RotationSpeed = 0.25f;
    private MeshFilter _mesh;

    private void Start()
    {
        _mesh = GetComponentInChildren<MeshFilter>();
    }

    private void Update()
    {
        _mesh.transform.Rotate(new Vector3(0.0f, 0.0f, RotationSpeed * 360.0f * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            try
            {
                Collect(other.GetComponentInParent<PlayerController>());
            }
            catch
            {
                throw new System.Exception("Cannot find PlayerController");
            }

            Destroy(this.gameObject);
        }
    }

    protected abstract void Collect(PlayerController player);
}
