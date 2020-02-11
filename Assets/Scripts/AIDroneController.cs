using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIDroneController : MonoBehaviour
{

    public bool DebugMode;

    private bool idle;
    private NavMeshAgent agent;
    private ObjectID objID;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        objID = GetComponent<ObjectID>();
        objID.objID = ObjectID.OBJECTID.UNIT;
    }

    void FixedUpdate()
    {
        //Check Idle Condition
        //If we are not moving
        idle = (agent.velocity.magnitude < 0.1f);
        



    }

}
