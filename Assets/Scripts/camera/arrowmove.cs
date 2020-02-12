using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowmove : MonoBehaviour
{
    private Vector3 newPos;
    private float zoom = 5.0f;
    public bool movable = true;
    public float speed = 1.0f;
    public LayerMask camwall;

 


 

    void Update()
    {

        if (movable == true)
        {
            newPos = Vector3.zero;
            newPos += Input.GetAxisRaw("Vertical") * new Vector3(1.0f, 0.0f, 1.0f);
            newPos += Input.GetAxisRaw("Horizontal") * new Vector3(1.0f, 0.0f, -1.0f);


            if (Input.mousePosition.y >= Screen.height)
            {
                newPos += new Vector3(1.0f, 0.0f, 1.0f);
            }
            if (Input.mousePosition.x >= Screen.width)
            {
                newPos += new Vector3(1.0f, 0.0f, -1.0f);
            }
            if (Input.mousePosition.y <= 1.0f)
            {
                newPos -= new Vector3(1.0f, 0.0f, 1.0f);
            }
            if (Input.mousePosition.x <= 1.0f)
            {
                newPos -= new Vector3(1.0f, 0.0f, -1.0f);
            }


            RaycastHit hitsphere;
            if (Physics.SphereCast(this.gameObject.transform.position, 0.5f, Vector3.forward, out hitsphere, Mathf.Infinity, camwall))
            {
                Debug.Log(hitsphere.collider.gameObject.name);
            }

            this.gameObject.transform.position += new Vector3(newPos.x, 0, newPos.z) * speed;



            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel > 0f)
            {
                zoom -= wheel * 2.0f;
            }
            else if (wheel < 0f)
            {
                Debug.LogError("Helo wolrd :D!!!!!");
                zoom -= wheel * 2.0f;
            }

            zoom = Mathf.Clamp(zoom, 1.0f, 30.0f);

            this.gameObject.GetComponent<Camera>().orthographicSize = zoom;



        }
    }
}
