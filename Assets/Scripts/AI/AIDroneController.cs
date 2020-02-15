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
            objID = tarObject.GetComponent<ObjectID>();
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
        public ObjectID objID;
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

    public enum UnitJob
    {
        WORKER,
        MINER,

    };

    //Publics
    public AttackState attackState;
    public bool DebugMode = false;
    public bool canMine = false;
    public bool canRepair = false;
    public float attackRange = 2.0f;
    public float attackDamage = 7.0f;
    public float agroRange = 10.0f;
    public float attackCooldown = 0.5f;
    public float miningRate = 3.0f;
    public float mineTime = 5.0f;
    public float mineMaxInv = 3.0f;
    public float repairTime = 5.0f;
    public float repairAmount = 1.0f;

    //Privates
    private NavMeshAgent agent;
    private ObjectID objID;
    private TargetObject target = null;
    private LayerMask interactLayer;
    private GameObject TC;
    private Vector3 TCdropOff;
    private bool idle;
    private bool stuck = false;
    private bool TCRetOverride = false;
    private float repairTimer = 0.0f;
    private float attackTimer = 0.0f;
    private float mineTimer = 0.0f;
    private float currentInv = 0.0f;

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
            target.tarObjectAdjustPos = GetAdjustedPos();
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

            //Crits for fun
            int dice = Random.Range(1, 13);
            if (dice == 12)
            {
                target.tarObject.GetComponent<ObjectID>().health -= attackDamage * (Random.Range(1.5f, 2.5f));
            }
            else
            {
                target.tarObject.GetComponent<ObjectID>().health -= attackDamage;
            }
            
            attackTimer = 0.0f;
        }
    }

    void TCRetOverrideBehaviour()
    {
        //Go To TC If not close enough
        TCdropOff = GetAdjustedPos(TC);
        if (Vector3.Distance(transform.position, TCdropOff) > attackRange)
        {
            NavMeshPath path = new NavMeshPath();
            if (idle)
            {
                agent.CalculatePath(TCdropOff, path);
                agent.SetPath(path);
                Resume();
            }
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

    void Repair()
    {
        Debug.Log("Repair Called");
        if (repairTimer >= repairTime)
        {
            if ((target.tarObject.GetComponent<ObjectID>().health + repairAmount) < target.tarObject.GetComponent<ObjectID>().maxHealth)
            {
                target.tarObject.GetComponent<ObjectID>().health += repairAmount;
            }
            else
            {
                target.tarObject.GetComponent<ObjectID>().health = target.tarObject.GetComponent<ObjectID>().maxHealth;
            }
            repairTimer = 0.0f;
        }
        //If repaired
        else if (target.tarObject.GetComponent<ObjectID>().health == target.tarObject.GetComponent<ObjectID>().maxHealth)
        {
            target.Reset();
        }
    }

    void Mine()
    {
        if (mineTimer >= mineTime)
        {
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
                mineTimer = 0.0f;
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

    Vector3 GetAdjustedPos()
    {
        Vector3 result = Vector3.zero;

        //Direction from the target to us
        Vector3 dir = (transform.position - target.tarObject.transform.position).normalized;

        //Predict movement if moving
        if (target.tarObject.GetComponent<ObjectID>().velo > 0.1f)
        {
            float time = Vector3.Distance(transform.position, target.tarObject.transform.position) / target.tarObject.GetComponent<ObjectID>().velo;

            float distanceMoved = time * target.tarObject.GetComponent<ObjectID>().velo;

            //Find target's position in future
            Vector3 futurePos = target.tarObject.transform.position + (target.tarObject.transform.forward.normalized * distanceMoved);

            //Direction from the target to us
            dir = (transform.position - futurePos).normalized;

            //Move the target position in regards to our attack range
            result = futurePos + (dir * attackRange);

        }
        else
        {
            //Move the target position in regards to our attack range
            result = target.tarObject.transform.position + (dir * attackRange);
        }



        return result;

    }


    Vector3 GetAdjustedPos(GameObject tar)
    {
        Vector3 result = Vector3.zero;

        //Direction from the target to us
        Vector3 dir = (transform.position - tar.transform.position).normalized;

        //Predict movement if moving
        if (target.tarObject.GetComponent<ObjectID>().velo > 0.1f)
        {
            float time = Vector3.Distance(transform.position, tar.transform.position) / tar.GetComponent<ObjectID>().velo;

            float distanceMoved = time * tar.GetComponent<ObjectID>().velo;

            //Find target's position in future
            Vector3 futurePos = tar.transform.position + (tar.transform.forward.normalized * distanceMoved);

            //Direction from the target to us
            dir = (transform.position - futurePos).normalized;

            //Move the target position in regards to our attack range
            result = futurePos + (dir * attackRange);

        }
        else
        {
            //Move the target position in regards to our attack range
            result = tar.transform.position + (dir * attackRange);
        }



        return result;

    }

    void AttackGameObject()
    {
        NavMeshPath path = new NavMeshPath();
        //Arrived at target object position
        if ((Vector3.Distance(transform.position, target.tarObjectAdjustPos) <= attackRange) || HasNeighbourStopped())
        {
            //If the target is within range
            if (Vector3.Distance(transform.position, target.tarObjectAdjustPos) <= attackRange)
            {
                Stop();
                if (target.tarObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.UNIT || target.tarObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.BUILDING)
                {
                    //If it is our building and we can repair
                    if (target.tarObject.GetComponent<ObjectID>().ownerPlayerID == objID.ownerPlayerID)
                    {
                        if (canRepair)
                        {
                            Repair();
                        }
                        else
                        {
                            target.Reset();
                        }
                    }
                    //enemy building/unit
                    else if ((target.tarObject.GetComponent<ObjectID>().ownerPlayerID != objID.ownerPlayerID && target.tarObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.BUILDING) || (target.tarObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.UNIT)){
                        DealDamage();
                    }
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
                    target.tarObjectAdjustPos = GetAdjustedPos();
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
                target.tarObjectAdjustPos = GetAdjustedPos();
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
                        target.tarObjectAdjustPos = GetAdjustedPos();

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
            target.tarObjectAdjustPos = GetAdjustedPos();

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

        objID.velo = agent.velocity.magnitude;

        //Timers
        attackTimer += Time.unscaledDeltaTime;
        mineTimer += Time.unscaledDeltaTime;
        repairTimer += Time.unscaledDeltaTime;

        //Check Idle Condition
        //If we are not moving
        idle = (agent.velocity.magnitude < 0.1f);

        //Stuck logic (path pending + no movement)
        stuck = (idle && (agent.pathPending || agent.pathStatus == NavMeshPathStatus.PathPartial));

        if (DebugMode)
        {
            //Debug.Log(stuck + "//" + agent.pathStatus + "||" + idle);
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
