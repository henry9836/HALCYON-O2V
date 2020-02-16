using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildingController : MonoBehaviour
{

    ObjectID objID;
    public float costToBuild = 25.0f;
    public bool built = false;
    public bool placed = false;
    public Material defaultMaterial;

    public void Placed()
    {
        gameObject.layer = LayerMask.NameToLayer("Building");
        GetComponent<NavMeshObstacle>().enabled = true;
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    private void Start()
    {
        objID = GetComponent<ObjectID>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (placed)
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
}
