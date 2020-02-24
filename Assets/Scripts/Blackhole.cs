﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    public float radius = 5.0f;
    public float power = 0.7f;

    private float minScale = 10.0f;
    private float maxscale = 300.0f;
    private float timer = 0.0f;
    private float twomintimer = 300.0f;

    public void Update()
    {
        timer += Time.deltaTime;
        float theScale = Mathf.Lerp(minScale, maxscale, timer/twomintimer);

        gameObject.transform.localScale = new Vector3(theScale, theScale, theScale);

        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - (gameObject.transform.localScale.x / 2.0f)));

    }

    public void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, Mathf.Infinity);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float gravityIntensity = 5.0f / Vector3.Distance(transform.position, hit.transform.position);
                hit.attachedRigidbody.AddForce((transform.position - hit.transform.position).normalized * gravityIntensity * hit.attachedRigidbody.mass * power * Time.deltaTime, ForceMode.Acceleration);
                //Debug.DrawRay(hit.transform.position, transform.position - hit.transform.position);
                if (Vector3.Distance(transform.position, hit.transform.position) < gameObject.transform.localScale.x / 2.0f)
                {
                    Destroy(hit.gameObject);
                }
            }
        }
    }
}