using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMe : MonoBehaviour
{

    public Vector3 lockVec;

    public bool lockX;
    public bool lockY;
    public bool lockZ;

    void Update()
    {
        if (lockX)
        {
            transform.rotation = Quaternion.Euler(new Vector3(lockVec.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        }
        if (lockY)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, lockVec.y, transform.rotation.eulerAngles.z));
        }
        if (lockZ)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, lockVec.z));
        }
    }
}
