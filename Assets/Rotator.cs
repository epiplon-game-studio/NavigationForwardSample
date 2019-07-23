using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 6;

    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, Time.deltaTime * speed, 0);
    }
}
