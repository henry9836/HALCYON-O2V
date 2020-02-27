using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouKeenForASmashy : MonoBehaviour
{

    public float damageVeloThreshold = 5.0f;

    private float mass = 1.0f;
    private float velo = 0.0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
        velo = rb.velocity.magnitude;
    }

    public void FixedUpdate()
    {
        if (rb == null)
        {
            Debug.LogWarning($"Rigidbody Missing From {gameObject.name}");
        }
        else
        {
            velo = rb.velocity.magnitude;
        }
    }

    //Use for Collison only requirements are rb/trigger on source and normal collider on other
    private void OnTriggerEnter(Collider other)
    {
        if (velo > damageVeloThreshold)
        {
            Debug.Log($"{gameObject.name} is keen for a smashy with {other.name}");
            if (other.GetComponent<ObjectID>())
            {
                other.GetComponent<ObjectID>().health -= (mass * 0.5f) * (velo * 0.5f);
                GetComponent<ObjectID>().health -= (mass * 0.5f) * (velo * 0.5f);
            }
        }
    }
}
