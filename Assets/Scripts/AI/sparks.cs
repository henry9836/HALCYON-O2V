using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sparks : MonoBehaviour
{
    public GameObject enabledparticle;

    public GameObject persent75;
    public GameObject persent50;
    public GameObject persent25;
    public GameObject persent0;

    private GameObject prefab75;
    private GameObject prefab50;
    private GameObject prefab25;
    private GameObject prefab0;

    public bool once75 = false;
    public bool once50 = false;
    public bool once25 = false;
    public bool once0 = false;



    void Update()
    {
        if (gameObject.GetComponent<ObjectID>().health / gameObject.GetComponent<ObjectID>().maxHealth < 0.75f)
        {
            if (once75 == false)
            {
                once75 = true;
                enabledparticle = persent75;
                prefab75 = Instantiate(enabledparticle, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                prefab75.transform.parent = this.gameObject.transform;
            }
        }
        if (gameObject.GetComponent<ObjectID>().health / gameObject.GetComponent<ObjectID>().maxHealth < 0.5f)
        {
            if (once50 == false)
            {
                Destroy(prefab75);
                once50 = true;
                enabledparticle = persent50;
                prefab50 = Instantiate(enabledparticle, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                prefab50.transform.parent = this.gameObject.transform;
            }
        }
        if (gameObject.GetComponent<ObjectID>().health / gameObject.GetComponent<ObjectID>().maxHealth < 0.25f)
        {
            if (once25 == false)
            {
                Destroy(prefab50);

                once25 = true;
                enabledparticle = persent25;
                prefab25 = Instantiate(enabledparticle, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                prefab25.transform.parent = this.gameObject.transform;
            }
        }
    }

    public void particleKill()
    {
        enabledparticle = persent0;
        prefab0 = Instantiate(enabledparticle, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        prefab0.transform.parent = null;
        Debug.Log("1");
        prefab0.GetComponent<sparks2>().call();
    }



}
