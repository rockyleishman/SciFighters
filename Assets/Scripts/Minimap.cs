using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Minimap : MonoBehaviour
{
    public Transform Player;
    private Vector3 newPosition;
    private void LateUpdate()
    {
        newPosition = Player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
