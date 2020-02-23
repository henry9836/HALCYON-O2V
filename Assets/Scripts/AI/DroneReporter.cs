using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneReporter : MonoBehaviour
{

    private AIDroneController ai;
    private ObjectID objID;
    private float attackRange = 10.0f;
    private LayerMask mask;
    private GameObject ownerAI;
    private AIBehaviour ownerAIBehaviour;

    public void iAmOwner(GameObject owner)
    {
        ownerAI = owner;
        ownerAIBehaviour = owner.GetComponent<AIBehaviour>();
    }

    private void Start()
    {
        ai = GetComponent<AIDroneController>();
        objID = GetComponent<ObjectID>();
        attackRange = ai.attackRange;
        mask = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().unitInteractLayers;
        //Do not spawn on player
        if (objID.ownerPlayerID == ObjectID.PlayerID.PLAYER)
        {
            Destroy(this);
        }

    }

    void Radar()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, attackRange);
        for (int i = 0; i < cols.Length; i++)
        {
            //If we don't own the object
            if (cols[i].GetComponent<ObjectID>().ownerPlayerID != objID.ownerPlayerID)
            {
                //Add to list
                ownerAIBehaviour.regNewSeenObject(cols[i].gameObject);
            }
        }

    }

    private void FixedUpdate()
    {
        Radar();
    }

}
