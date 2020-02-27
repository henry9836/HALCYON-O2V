﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    public float radius = 5.0f;
    public float power = 0.7f;

    private float minScale = 10.0f;
    private float maxscale = 300.0f;
    public float timer = 0.0f;
    public float twomintimer = 420.0f;

    public void Update()
    {
        timer += Time.unscaledDeltaTime;
        float theScale = Mathf.Lerp(minScale, maxscale, timer/twomintimer);

        gameObject.transform.localScale = new Vector3(theScale, theScale, theScale);

    }

    public void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, Mathf.Infinity);
        foreach (Collider hit in colliders)
        {

            if (hit.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                if (Vector3.Distance(transform.position, hit.transform.position) < gameObject.transform.localScale.x / 2.0f)
                {
                    Destroy(hit.gameObject);
                }
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                if ((Vector3.Distance(hit.transform.position, transform.position) - (gameObject.transform.localScale.x / 2.0f)) < 20.0f)
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        float gravityIntensity = 5.0f / Vector3.Distance(transform.position, hit.transform.position);
                        hit.attachedRigidbody.AddForce((transform.position - hit.transform.position).normalized * gravityIntensity * hit.attachedRigidbody.mass * power * Time.deltaTime, ForceMode.Acceleration);

                    }

                    if (Vector3.Distance(transform.position, hit.transform.position) < gameObject.transform.localScale.x / 2.0f)
                    {
                        if (hit.gameObject.tag == "TC")
                        {
                            if (hit.gameObject.GetComponent<ObjectID>().ownerPlayerID == ObjectID.PlayerID.PLAYER)
                            {
                                hit.gameObject.GetComponent<TCController>().gameLossUI.SetActive(true);
                                Debug.Log("deaded");
                            }
                        }
                        Destroy(hit.gameObject);
                    }
                }

            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Resource"))
            {

                Rigidbody rb = hit.GetComponent<Rigidbody>();

                float gravityIntensity = 5.0f / Vector3.Distance(transform.position, hit.transform.position);
                hit.attachedRigidbody.AddForce((transform.position - hit.transform.position).normalized * gravityIntensity * hit.attachedRigidbody.mass * power * Time.deltaTime, ForceMode.Acceleration);


                if (Vector3.Distance(transform.position, hit.transform.position) < gameObject.transform.localScale.x / 2.0f)
                {

                    Destroy(hit.gameObject);
                }
            }
        }
    }
}
