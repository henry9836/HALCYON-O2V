using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turret : MonoBehaviour
{
    public float dps;
    public Vector2 minmaxRange;
    private GameObject HP;
    private LayerMask mask;

    

    void Start()
    {
        HP = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        mask = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().unitInteractLayers;
    }

    void Update()
    {
        hpupdate();
        pewpew();
    }

    void hpupdate()
    {
        HP.GetComponent<Image>().fillAmount = GetComponent<ObjectID>().health / GetComponent<ObjectID>().maxHealth;
    }

    void pewpew()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, minmaxRange.y, mask);

        float closest = 9999999.0f;
        GameObject target = null;

        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.layer != LayerMask.NameToLayer("Resource"))
                {
                    if (hitColliders[i].gameObject.GetComponent<ObjectID>().ownerPlayerID != gameObject.GetComponent<ObjectID>().ownerPlayerID) 
                    {
                        float dist = Vector3.Distance(gameObject.transform.position, hitColliders[i].transform.position);
                        if (dist > minmaxRange.x)
                        {
                            if (dist < closest)
                            {
                                target = hitColliders[i].gameObject;
                                closest = dist;
                            }
                        }
                    }

                }


            }
        }

        if (target != null)
        {
            target.GetComponent<ObjectID>().health -= dps * Time.deltaTime;
            gameObject.GetComponent<LineRenderer>().enabled = true;
            gameObject.GetComponent<LineRenderer>().SetPosition(0, transform.gameObject.transform.position);
            gameObject.GetComponent<LineRenderer>().SetPosition(1, target.transform.position);

        }
        else
        {
            gameObject.GetComponent<LineRenderer>().enabled = false;

        }
    }
}
