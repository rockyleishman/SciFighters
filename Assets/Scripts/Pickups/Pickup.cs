using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    [SerializeField] public float MagneticAcceleration = 10.0f;
    private float _magneticSpeed;
    private bool _isMagnetized;
    private PlayerController _player;

    [SerializeField] public float RotationSpeed = 0.25f;
    [SerializeField] public bool RotationAxisY = false;
    private MeshFilter _mesh;

    private void Start()
    {
        _mesh = GetComponentInChildren<MeshFilter>();

        _isMagnetized = false;
        _magneticSpeed = 0;
    }

    private void Update()
    {
        if (RotationAxisY)
        {
            _mesh.transform.Rotate(new Vector3(0.0f, RotationSpeed * 360.0f * Time.deltaTime, 0.0f));
        }
        else
        {
            //rotate on Z
            _mesh.transform.Rotate(new Vector3(0.0f, 0.0f, RotationSpeed * 360.0f * Time.deltaTime));
        }

        if (_isMagnetized)
        {
            //move towards collector
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position + new Vector3(0.0f, _player.BodyHeight / 2.0f, 0.0f), _magneticSpeed * Time.deltaTime);

            //increase magnet speed
            _magneticSpeed = (_magneticSpeed + MagneticAcceleration * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            try
            {
                _player = other.GetComponentInParent<PlayerController>();
                _isMagnetized = true;
            }
            catch
            {
                throw new System.Exception("Cannot find PlayerController on tagged \"Player\"");
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            try
            {
                Collect(collision.collider.GetComponentInParent<PlayerController>());
                Destroy(this.gameObject);
            }
            catch
            {
                throw new System.Exception("Cannot find PlayerController on tagged \"Player\"");
            }
        }
    }

    protected abstract void Collect(PlayerController player);
}