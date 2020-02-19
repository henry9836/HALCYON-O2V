using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class astroidcurve : MonoBehaviour
{
    public Vector3 force;
    void FixedUpdate()
    {
        this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
    }


}
