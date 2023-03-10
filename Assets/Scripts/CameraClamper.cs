using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamper : MonoBehaviour
{
    //this script should be attached to the camera pivot

    [SerializeField] [Range(0.0f, 180.0f)] public float UpperClamp = 45.0f;
    [SerializeField] [Range(-180.0f, 0.0f)] public float LowerClamp = -45.0f;

    void LateUpdate()
    {
        //clamp up/down movement
        if (transform.rotation.eulerAngles.x > UpperClamp && transform.rotation.eulerAngles.x < 180.0f)
        {
            transform.rotation = Quaternion.Euler(UpperClamp, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        else if (transform.rotation.eulerAngles.x < LowerClamp + 360.0f && transform.rotation.eulerAngles.x > 180.0f)
        {
            transform.rotation = Quaternion.Euler(LowerClamp + 360.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
