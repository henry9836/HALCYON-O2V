using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowmove : MonoBehaviour
{
    private Vector3 newPos;
    private float zoom = 5.0f;
    public bool movable = true;
    public float speed = 1.0f;
    public LayerMask grid;


    //horozontal x = x value when most left, y = z value when most right
    //vertical x = y value when most down, y = y when most up
    public Vector2 horozontalbounds;
    public Vector2 verticalbounds;


    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, grid))
        {
            if (hit.distance < 40.0f)
            {
                this.transform.position += new Vector3(-5.0f, 0.0f, -5.0f);
            }
        }

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

            if (newPos.x < 0.0f)
            {
                if (this.gameObject.transform.position.x >= horozontalbounds.x)
                {
                    this.gameObject.transform.position += new Vector3(newPos.x, 0, newPos.z) * speed;
                }
            }
            
            if (newPos.x > 0.0f)
            {
                if (this.gameObject.transform.position.z >= horozontalbounds.y)
                {
                    this.gameObject.transform.position += new Vector3(newPos.x, 0, newPos.z) * speed;
                }
            } 
            
            if (newPos.y < 0.0f)
            {
                if (this.gameObject.transform.position.y >= verticalbounds.x)
                {
                    this.gameObject.transform.position += new Vector3(0.0f, newPos.y, 0.0f) * speed;
                }
            }
            
            if (newPos.y > 0.0f)
            {
                if (this.gameObject.transform.position.y <= verticalbounds.y)
                {
                    this.gameObject.transform.position += new Vector3(0.0f, newPos.y, 0.0f) * speed;
                }
            }


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
            this.gameObject.GetComponent<Camera>().orthographicSize = Mathf.Clamp(zoom, 1.0f, 30.0f);



        }
    }
}
