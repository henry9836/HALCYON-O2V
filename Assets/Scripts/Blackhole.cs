using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    public float radius = 5.0f;
    public float power = 0.7f;

    public void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float gravityIntensity = Vector3.Distance(transform.position, hit.transform.position) / 5.0f;
                hit.attachedRigidbody.AddForce((transform.position - hit.transform.position) * gravityIntensity * hit.attachedRigidbody.mass * power * Time.smoothDeltaTime);
                //Debug.DrawRay(hit.transform.position, transform.position - hit.transform.position);

                if (Vector3.Distance(transform.position, hit.transform.position) < 1.0f)
                {
                    Destroy(hit.gameObject);
                
                }
            }
        }
    }
}
