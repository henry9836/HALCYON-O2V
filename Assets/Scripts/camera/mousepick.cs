using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousepick : MonoBehaviour
{
    public LayerMask gridmask;

    public Vector3 getMousePos()
    {
        Ray hitpoint = this.gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(hitpoint.origin, hitpoint.direction, out hit, Mathf.Infinity, gridmask))
        {
            Debug.DrawLine(hit.point, hitpoint.origin);
            return (hit.point);
        }

        //make a bigger grid so this isnt run
        Debug.LogWarning("mosue outside map");
        return (Vector3.zero);
    }


}
