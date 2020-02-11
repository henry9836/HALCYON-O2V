using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowmove : MonoBehaviour
{
    private Vector3 newPos;
    private float zoom = 5.0f;
    public bool movable = true;

    public float speed = 1.0f;
    void Update()
    {
        if (movable == true)
        {
            newPos = Vector3.zero;
            newPos += Input.GetAxisRaw("Vertical") * Vector3.up;
            newPos += Input.GetAxisRaw("Horizontal") * new Vector3(1.0f, 0.0f, -1.0f);


            if (Input.mousePosition.y >= Screen.height)
            {
                newPos += Vector3.up;
            }
            if (Input.mousePosition.x >= Screen.width)
            {
                newPos += new Vector3(1.0f, 0.0f, -1.0f);
            }
            if (Input.mousePosition.y <= 1.0f)
            {
                newPos -= Vector3.up;
            }
            if (Input.mousePosition.x <= 1.0f)
            {
                newPos -= new Vector3(1.0f, 0.0f, -1.0f);
            }

            this.gameObject.transform.position += newPos * speed;

            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel > 0f)
            {
                zoom -= wheel * 2.0f;
            }
            else if (wheel < 0f)
            {
                zoom -= wheel * 2.0f;
            }
            this.gameObject.GetComponent<Camera>().orthographicSize = zoom;



        }
    }
}
