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





    void FixedUpdate()
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
                //up wall
                if (hitsphere[i].gameObject.name == FBLR[0].name)
                {
                    //holding up 
                    if ((newPos.x > 0 && newPos.z > 0)  )
                    {
                        newPos = Vector3.zero;
                    }

                    //holding up and left
                    if ((newPos.x > 1))
                    {
                        newPos = Vector3.zero;
                        newPos += new Vector3(1.0f, 0.0f, -1.0f);
                        if (check(hitsphere, 0, 2) == true)
                        {
                            newPos = Vector3.zero;
                        }
                    }

                    //holding up and right
                    if ((newPos.z > 1))
                    {
                        newPos = Vector3.zero;
                        newPos -= new Vector3(1.0f, 0.0f, -1.0f);
                        if (check(hitsphere, 0, 3) == true)
                        {
                            newPos = Vector3.zero;
                        }
                    }
                }


                //back wall
                else if (hitsphere[i].gameObject.name == FBLR[1].name)
                {
                    //holdin down
                    if (newPos.x < 0 && newPos.z < 0)
                    {
                        newPos = Vector3.zero;
                    }


                    //down and right
                    if ((newPos.x < -1))
                    {
                        newPos = Vector3.zero;
                        newPos -= new Vector3(1.0f, 0.0f, -1.0f);
                        if (check(hitsphere, 1, 3) == true)
                        {
                            newPos = Vector3.zero;
                        }
                    }

                    //down and left
                    if ((newPos.z < -1))
                    {
                        newPos = Vector3.zero;
                        newPos += new Vector3(1.0f, 0.0f, -1.0f);
                        if (check(hitsphere, 1, 2) == true)
                        {
                            newPos = Vector3.zero;
                        }

                    }
                }

                else if (hitsphere[i].gameObject.name == FBLR[2].name)
                {
                    //holding left
                    if (newPos.x < 0 && newPos.z > 0)
                    {
                        newPos = Vector3.zero;
                    }

                    //left and down
                    if (newPos.x < -1)
                    {
                        newPos = Vector3.zero;
                        newPos -= new Vector3(1.0f, 0.0f, 1.0f);
                        if (check(hitsphere, 2, 2) == true)
                        {
                            newPos = Vector3.zero;
                        }
                    }

                    //left amd up 
                    if (newPos.z > 1)
                    {
                        newPos = Vector3.zero;
                        newPos += new Vector3(1.0f, 0.0f, 1.0f);
                        if (check(hitsphere, 2, 1) == true)
                        {
                            newPos = Vector3.zero;
                        }
                    }
                }

                else if (hitsphere[i].gameObject.name == FBLR[3].name)
                {
                    //holding right
                    if (newPos.x > 0 && newPos.z < 0)
                    {
                        newPos = Vector3.zero;
                    }


                    //right and up 
                    if (newPos.x > 1)
                    {
                        newPos = Vector3.zero;
                        newPos += new Vector3(1.0f, 0.0f, 1.0f);
                        if (check(hitsphere, 3, 1) == true)
                        {
                            newPos = Vector3.zero;
                        }
                    }

                    //right and down
                    if (newPos.z < -1)
                    {
                        newPos = Vector3.zero;
                        newPos -= new Vector3(1.0f, 0.0f, 1.0f);
                        if (check(hitsphere, 3, 2) == true)
                        {
                            newPos = Vector3.zero;
                        }
                    }
                }
            }


            this.gameObject.transform.position += Vector3.ClampMagnitude(new Vector3(newPos.x, 0, newPos.z) , 1.0f) * speed * (zoom);



            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel > 0f)
            {
                zoom -= wheel * 2.0f;
            }
            else if (wheel < 0f)
            {
                zoom -= wheel * 2.0f;
            }

            zoom = Mathf.Clamp(zoom, 1.0f, 30.0f);

            this.gameObject.GetComponent<Camera>().orthographicSize = zoom;



        }
    }

    bool check(Collider[] hitsphere, int core, int excpetion)
    {
        bool thecheck = false;
        for (int i = 0; i < hitsphere.Length; i++)
        {
            if ((hitsphere[i].gameObject.name == FBLR[0].name) && core != 0 && excpetion != 0)
            {
                thecheck = true;
            }
            if ((hitsphere[i].gameObject.name == FBLR[1].name) && core != 1 && excpetion != 1)
            {
                thecheck = true;
            }
            if ((hitsphere[i].gameObject.name == FBLR[2].name) && core != 2 && excpetion != 2)
            {
                thecheck = true;
            }
            if ((hitsphere[i].gameObject.name == FBLR[3].name) && core != 3 && excpetion != 3)
            {
                thecheck = true;
            }
        }
        return (thecheck);
    }
}
