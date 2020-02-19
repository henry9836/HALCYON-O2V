using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    ObjectID objID;

    void Start()
    {
        objID = GetComponent<ObjectID>();
    }
    
    void FixedUpdate()
    {
        if (objID.health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
