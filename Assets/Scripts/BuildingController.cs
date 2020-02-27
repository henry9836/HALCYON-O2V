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

    private NavMeshAgent agent;
    private NavMeshPath path;

    public void Placed()
    {
        gameObject.layer = LayerMask.NameToLayer("Building");
        if (GetComponent<NavMeshObstacle>() != null)
        {
            GetComponent<NavMeshObstacle>().enabled = true;
        }
        if (GetComponent<MeshRenderer>())
        {
            GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
    }

    private void Start()
    {
        objID = GetComponent<ObjectID>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (placed)
        {
            //We have been built
            if (!built && objID.health == objID.maxHealth)
            {
                built = true;
            }
        }

        //Health Speical Effects
        


    }
}
