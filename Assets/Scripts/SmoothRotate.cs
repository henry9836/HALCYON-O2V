using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRotate : MonoBehaviour
{

    public float speed = 1.0f;
    public Vector3 rot = Vector3.up;

    private void Start()
    {
        rot = rot.normalized;    
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (rot * speed * Time.deltaTime));
    }
}
