using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouKeenForASmashy : MonoBehaviour
{

    private float mass = 1.0f;
    private float velo = 0.0f;
    private Rigidbody rb;

    private void Start()
    {
        rb.GetComponent<Rigidbody>();
        mass = rb.mass;
        velo = rb.velocity.magnitude;
    }

    public void FixedUpdate()
    {
        velo = rb.velocity.magnitude;
    }

    //Use for Collison only requirements are rb/trigger on source and normal collider on other
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{gameObject.name} is keen for a smashy with {other.name}");
        if (other.GetComponent<ObjectID>())
        {
            other.GetComponent<ObjectID>().health -= mass * velo;
            GetComponent<ObjectID>().health -= mass * velo;
        }
    }
}
