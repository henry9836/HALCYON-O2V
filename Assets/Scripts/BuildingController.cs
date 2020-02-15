using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    ObjectID objID;
    public bool built = false;

    private void Start()
    {
        objID = GetComponent<ObjectID>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (objID.health <= 0)
        {
            Destroy(gameObject);
        }

        //We have been built
        if (!built && objID.health == objID.maxHealth)
        {
            built = true;
        }

    }
}
