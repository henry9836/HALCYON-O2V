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

    public List<GameObject> FBLR;


 

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


            Collider[] hitsphere = Physics.OverlapSphere(this.gameObject.transform.position, 1.0f, camwall);

            for (int i = 0; i < hitsphere.Length; i++)
            {
                Debug.Log(hitsphere[i].gameObject.name);
                if (hitsphere[i].gameObject.name == FBLR[0].name)
                {
                    if (newPos.x > 0 && newPos.z > 0)
                    {
                        newPos = Vector3.zero;
                    }
                }
                if (hitsphere[i].gameObject.name == FBLR[1].name)
                {
                    if (newPos.x < 0 && newPos.z < 0)
                    {
                        newPos = Vector3.zero;
                    }
                }
                if (hitsphere[i].gameObject.name == FBLR[2].name)
                {
                    if (newPos.x < 0 && newPos.z > 0)
                    {
                        newPos = Vector3.zero;
                    }
                }
                if (hitsphere[i].gameObject.name == FBLR[3].name)
                {
                    if (newPos.x > 0 && newPos.z < 0)
                    {
                        newPos = Vector3.zero;
                    }
                }
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
