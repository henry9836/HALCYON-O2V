using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIDroneController : MonoBehaviour
{
    //Classes
    public class TargetObject
    {
        public TargetObject(GameObject obj)
        {
            tarObject = obj;
        }

        public TargetObject(Vector3 pos)
        {
            tarPos = pos;
        }

        public bool hasTarget()
        {
            return (tarObject != null || tarPos != Vector3.zero);
        }

        public bool hasTargetPos()
        {
            return (tarPos != Vector3.zero);
        }

        public bool hasTargetObj()
        {
            return (tarObject != null);
        }

        public void Reset()
        {
            tarPos = Vector3.zero;
            tarObject = null;
            tarObjectAdjustPos = Vector3.zero;
        }

        public GameObject tarObject = null;
        public Vector3 tarObjectAdjustPos = Vector3.zero;
        public Vector3 tarPos = Vector3.zero;
    };

    public enum AttackState
    {
        ATTACK,
        STANDGROUND,
        NOATTACK
    };

    //Publics
    public bool DebugMode;
    public float attackRange = 2.0f;
    public float attackDamage = 7.0f;
    public float agroRange = 10.0f;
    public float attackCooldown = 0.5f;

    //Privates
    private bool idle;
    private NavMeshAgent agent;
    private ObjectID objID;
    private TargetObject target = null;
    private AttackState attackState;
    private LayerMask interactLayer;
    private NavMeshPath path = new NavMeshPath();
    private bool stuck = false;
    private float attackTimer = 0.0f;

    public bool isAIStopped()
    {
        return agent.isStopped;
    }

    void Stop()
    {
        agent.isStopped = true;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        objID = GetComponent<ObjectID>();
        objID.objID = ObjectID.OBJECTID.UNIT;
        interactLayer = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().unitInteractLayers;
    }
    
    void DealDamage()
    {
        if (attackTimer > attackCooldown)
        {
            target.tarObject.GetComponent<ObjectID>().health -= attackDamage;
        }
    }

    bool HasNeighbourStopped()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, attackRange, interactLayer);

        for (int i = 0; i < cols.Length; i++)
        {
            //Target our allys
            if (cols[i].gameObject.GetComponent<ObjectID>().ownerPlayerID == objID.ownerPlayerID)
            {
                //Is an AI Object
                if (cols[i].GetComponent<AIDroneController>() != null)
                {
                    if (cols[i].GetComponent<AIDroneController>().isAIStopped())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void AttackGameObject()
    {
        //Get to the enemy
        if (Vector3.Distance(transform.position, target.tarObjectAdjustPos) > attackRange)
        {
            path = new NavMeshPath();
            //Go To Point
            agent.CalculatePath(target.tarObjectAdjustPos, path);
            agent.SetPath(path);
        }

        //Arrived
        else if ((Vector3.Distance(transform.position, target.tarObjectAdjustPos) <= attackRange) || HasNeighbourStopped())
        {
            if (Vector3.Distance(transform.position, target.tarObjectAdjustPos) <= attackRange)
            {
                Stop();
                DealDamage();
            }
            //We are not close enough keep going
        }

        //Stuck
        if (stuck)
        {
            Stop();
        }
    }

    void GoToTargetPos()
    {
        //If we are not within range of our target pos
        if (Vector3.Distance(transform.position, target.tarPos) > attackRange)
        {
            path = new NavMeshPath();
            //Go To Point
            agent.CalculatePath(target.tarPos, path);
            agent.SetPath(path);
        }

        //Stuck
        if (stuck)
        {
            Stop();
        }
        
    }

    void AttackLogic()
    {
        if (target.hasTargetObj())
        {
            AttackGameObject();
        }
        else if (target.hasTargetPos())
        {
            GoToTargetPos();
        }
    }

    bool FindTarget()
    {

        Collider[] cols;

        if (attackState == AttackState.ATTACK)
        {
            cols = Physics.OverlapSphere(transform.position, agroRange, interactLayer);
        }
        else
        {
            cols = Physics.OverlapSphere(transform.position, attackRange, interactLayer);
        }

        for (int i = 0; i < cols.Length; i++)
        {
            //Ignore our own objects
            if (cols[i].gameObject.GetComponent<ObjectID>().ownerPlayerID != objID.ownerPlayerID)
            {
                //Make a new Target
                target = new TargetObject(cols[i].gameObject);

                //Get a good position near to the target in a arc realitve to our position

                return true;
            }
        }

        return false;
    }

    void IdleAttackLogic()
    {
        if (attackState == AttackState.ATTACK)
        {
            if (!target.hasTarget())
            {
                FindTarget();
            }
        }
        else if (attackState == AttackState.STANDGROUND)
        {
            FindTarget();
        }
        //NO ATTACK
        else
        {
            return;
        }
    }

    void FixedUpdate()
    {
        attackTimer += Time.unscaledDeltaTime;

        //Check Idle Condition
        //If we are not moving
        idle = (agent.velocity.magnitude < 0.1f);

        //Stuck logic (path pending + no movement)
        stuck = (idle && agent.pathPending);

        if (idle && !target.hasTarget())
        {
            IdleAttackLogic();
        }

        if (target.hasTarget())
        {
            AttackLogic();
        }
        
        //Death
        if (objID.health <= 0)
        {
            Destroy(gameObject);
        }


    }

}
