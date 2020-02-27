using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camtargetminimap : MonoBehaviour
{
    private GameObject cam;
    public LayerMask gridmask;

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").gameObject;
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, gridmask))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward * hit.distance, Color.yellow);
        }

        this.gameObject.transform.position = new Vector3(hit.point.x, hit.point.y + 300.0f, hit.point.z);
    }
}
