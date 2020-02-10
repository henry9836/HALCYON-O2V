using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowmove : MonoBehaviour
{
    public Vector3 newPos;
    void Update()
    {
        newPos = Vector3.zero;
        newPos += Input.GetAxisRaw("Vertical") * Vector3.up;
        newPos += Input.GetAxisRaw("Horizontal") * new Vector3(1.0f, 0.0f, -1.0f);
        this.gameObject.transform.position += newPos;
    }
}
