﻿using System.Collections;
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
        public void Sanity()
        {
            if (tarObject == null)
            {
                tarObjectAdjustPos = Vector3.zero;
            }
        }

        public GameObject tarObject = null;
        public Vector3 tarObjectAdjustPos = Vector3.zero;
        public Vector3 tarPos = Vector3.zero;
    };

    public class aiDebug
    {
        public aiDebug(TargetObject _t, bool _s, bool _i, AttackState _a)
        {
            target = _t;
            stuck = _s;
            idle = _i;
            attackState = _a;
        }

        public TargetObject target;
        public bool stuck = false;
        public bool idle = false;
        public AttackState attackState;
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
    public bool canMine = false;
    public float miningRate = 3.0f;
    public float mineTime = 5.0f;
    public float mineMaxInv = 3.0f;
    public AttackState attackState;

    //Privates
    private bool idle;
    private NavMeshAgent agent;
    private ObjectID objID;
    private TargetObject target = null;
    private LayerMask interactLayer;
    private bool stuck = false;
    private float attackTimer = 0.0f;
    private float mineTimer = 0.0f;
    private float currentInv = 0.0f;
    private bool TCRetOverride = false;
    private GameObject TC;

    public aiDebug returnDebug()
    {
        return new aiDebug(target, stuck, idle, attackState);
    }


    public bool isAIStopped()
    {
        return agent.isStopped;
    }

    public void UpdateTargetPos(Vector3 targetPos, GameObject obj)
    {
        if (obj != null)
        {
            target = new TargetObject(obj);

            //Find a position
            target.tarObjectAdjustPos = target.tarObject.transform.position;
        }
        else {
            target = new TargetObject(targetPos);
        }
    }

    void Stop()
    {
        agent.isStopped = true;
        agent.ResetPath();
        Resume();
    }

    void Resume()
    {
        agent.isStopped = false;
    }

    void FindTC()
    {
        GameObject[] tcs = GameObject.FindGameObjectsWithTag("TC");
        for (int i = 0; i < tcs.Length; i++)
        {
            if (tcs[i].GetComponent<ObjectID>().ownerPlayerID == GetComponent<ObjectID>().ownerPlayerID)
            {
                TC = tcs[i];
            }
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        objID = GetComponent<ObjectID>();
        objID.objID = ObjectID.OBJECTID.UNIT;
        interactLayer = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().unitInteractLayers;
        target = new TargetObject(Vector3.zero);
        FindTC();
    }
    
    void DealDamage()
    {
        if (attackTimer > attackCooldown)
        {
            Debug.DrawLine(transform.position, target.tarObject.transform.position, Color.red, 1.0f);

            target.tarObject.GetComponent<ObjectID>().health -= attackDamage;
            attackTimer = 0.0f;
        }
    }

    void TCRetOverrideBehaviour()
    {
        //Go To TC If not close enough
        if (Vector3.Distance(transform.position, TC.transform.position) > attackRange)
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(target.tarObjectAdjustPos, path);
            agent.SetPath(path);
            Resume();
        }

        //Deposit and toggle off override
        else
        {
            if (currentInv > 0)
            {
                //Find gamemanager and update resources
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().UpdateResourceCount((int)objID.ownerPlayerID, currentInv);
                currentInv = 0;
            }
            TCRetOverride = false;
        }

    }

    void Mine()
    {
        Debug.Log("Mine() called");
        Debug.Log(mineTime + "/" + mineTimer);
        if (mineTime >= mineTimer)
        {
            Debug.Log("We in");
            if (currentInv >= mineMaxInv)
            {
                //Go deposit inv at TC
                TCRetOverride = true;

            }
            else
            {
                //Mine some more
                target.tarObject.GetComponent<ObjectID>().health -= attackDamage;
                currentInv += miningRate;
                attackTimer = 0.0f;
            }
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
        NavMeshPath path = new NavMeshPath();
        //Arrived at target object position
        if ((Vector3.Distance(transform.position, target.tarObjectAdjustPos) <= attackRange) || HasNeighbourStopped())
        {
            //If the target is within range
            if (Vector3.Distance(transform.position, target.tarObject.transform.position) <= attackRange)
            {
                Debug.LogWarning("Arrived" + gameObject.name);
                Stop();
                if (target.tarObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.UNIT || target.tarObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.BUILDING)
                {
                    Debug.LogWarning("FUCK");
                    DealDamage();
                }
                else if (target.tarObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.RESOURCE && canMine)
                {
                    Mine();
                }
                else
                {
                    Debug.LogWarning("Unspecified action for target {" + target.tarObject.GetComponent<ObjectID>().objID + "}");
                }
            }
            else
            {
                if ((Vector3.Distance(transform.position, target.tarObject.transform.position) > attackRange))
                {
                    Debug.LogWarning("Get To Enemy my friend stopped");
                    target.tarObjectAdjustPos = target.tarObject.transform.position;
                    agent.CalculatePath(target.tarObjectAdjustPos, path);
                    agent.SetPath(path);
                    Resume();
                }
            }
        }

        //Get to the enemy
        if (Vector3.Distance(transform.position, target.tarObject.transform.position) > attackRange)
        {
            Debug.LogWarning("Get To Enemy");
            //Go To Point
            //If the adjusted object pos is further away from the tar obj then we can hit then get a new one
            if (Vector3.Distance(target.tarObject.transform.position, target.tarObjectAdjustPos) > attackRange)
            {
                target.tarObjectAdjustPos = target.tarObject.transform.position;
                Resume();
            }
            agent.CalculatePath(target.tarObjectAdjustPos, path);
            agent.SetPath(path);
        }

        //Stuck
        if (stuck)
        {
            Debug.LogWarning("Stuck");
            Stop();
        }
    }

    void GoToTargetPos()
    {
        NavMeshPath path = new NavMeshPath();

        //If we are not within range of our target pos
        if (Vector3.Distance(transform.position, target.tarPos) > attackRange)
        {
            path = new NavMeshPath();
            //Go To Point
            agent.CalculatePath(target.tarPos, path);
            agent.SetPath(path);
        }

        //Arrived
        if (Vector3.Distance(transform.position, target.tarPos) < 1.0f || HasNeighbourStopped())
        {
            Debug.LogWarning("Arrived" + gameObject.name);
            Stop();
            target.Reset();
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

        List<GameObject> foundResources = new List<GameObject>();

        for (int i = 0; i < cols.Length; i++)
        {

            
            //Ignore our own objects
            if (cols[i].gameObject.GetComponent<ObjectID>() != null)
            {
                if (cols[i].gameObject.GetComponent<ObjectID>().ownerPlayerID != objID.ownerPlayerID)
                {
                    //If is resource and cannot mine
                    if (!canMine && cols[i].gameObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.RESOURCE) {
                        //nothing
                    }
                    //Add to foundResources
                    else if (canMine && cols[i].gameObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.RESOURCE)
                    {
                        foundResources.Add(cols[i].gameObject);
                    }
                    //Not a resource is a unit
                    else
                    {
                        //Make a new Target
                        target = new TargetObject(cols[i].gameObject);
                        target.tarObjectAdjustPos = target.tarObject.transform.position;

                        //Get a good position near to the target in a arc realitve to our position

                        return true;
                    }
                }
            }
        }

        //Fallback onto resources
        if (foundResources.Count > 0)
        {
            float shortestDis = Mathf.Infinity;
            GameObject shortestDirObj = null;

            for (int i = 0; i < foundResources.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, foundResources[i].transform.position);
                if (distance < shortestDis)
                {
                    shortestDis = distance;
                    shortestDirObj = foundResources[i];
                }
            }

            target = new TargetObject(shortestDirObj);
            target.tarObjectAdjustPos = target.tarObject.transform.position;

            return true;
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
        if (TC == null)
        {
            FindTC();
        }

        attackTimer += Time.unscaledDeltaTime;

        //Check Idle Condition
        //If we are not moving
        idle = (agent.velocity.magnitude < 0.1f);

        //Stuck logic (path pending + no movement)
        stuck = (idle && (agent.pathPending || agent.pathStatus == NavMeshPathStatus.PathPartial));

        if (DebugMode)
        {
            Debug.Log(stuck + "//" + agent.pathStatus + "||" + idle);
        }

        target.Sanity();

        if (TCRetOverride)
        {
            TCRetOverrideBehaviour();
        }
        else
        {
            if (idle && !target.hasTarget())
            {
                IdleAttackLogic();
            }

            if (target.hasTarget())
            {
                AttackLogic();
            }
        }
        //Death
        if (objID.health <= 0)
        {
            Destroy(gameObject);
        }


    }


}
