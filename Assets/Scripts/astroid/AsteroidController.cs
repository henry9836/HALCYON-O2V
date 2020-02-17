using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    private ObjectID objID;

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
        objID = GetComponent<ObjectID>();
    }
    
    void FixedUpdate()
    {
        if (objID.health <= 0)
        {
            Destroy(gameObject);
        }

        float scale = Mathf.Lerp(Vector3.zero.x, startScale.x, (objID.health / objID.maxHealth));

        Debug.Log(scale);

        transform.localScale = new Vector3(scale, scale, scale);

    }
}
