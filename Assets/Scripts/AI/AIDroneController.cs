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
                Reset();
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

    public enum DroneMode
    {
        WORKER,
        FIGHTER,
        MINER,
        BOMBER,
        BOOSTER
    };

    //Publics
    public List<GameObject> laserEmitterPositions = new List<GameObject>();
    public AttackState attackState = AttackState.ATTACK;
    public DroneMode droneMode = DroneMode.WORKER;
    public bool DebugMode = false;
    public bool canMine = false;
    public bool canRepair = false;
    public bool canFight = false;
    public bool bomber = false;
    public bool canAttachTo = false;
    public bool aiLock = false;
    public bool asteriodOverride = false;
    public float attackRange = 2.0f;
    public float attackDamage = 7.0f;
    public float agroRange = 10.0f;
    public float attackCooldown = 0.5f;
    public float miningRate = 3.0f;
    public float mineTime = 5.0f;
    public float mineMaxInv = 3.0f;
    public float repairTime = 5.0f;
    public float repairAmount = 1.0f;
    public float laserSustainTime = 0.3f;
    public Rigidbody asteriodBody;

    //Privates
    private GameManager GM;
    private NavMeshAgent agent;
    private ObjectID objID;
    private TargetObject target = null;
    private LineRenderer lr;
    private GameObject TC;
    private LayerMask interactLayer;
    private Vector3 TCdropOff;
    private bool idle;
    private bool stuck = false;
    private bool TCRetOverride = false;
    private int currentLaserEmitter = 0;
    private float repairTimer = 0.0f;
    private float attackTimer = 0.0f;
    private float mineTimer = 0.0f;
    private float currentInv = 0.0f;
    private float idleTimer = 0.0f;
    private float maxstuckTime = 5.0f;
    private float orignialAttackRange = 2.0f;
    private float orignialAttackDamage = 7.0f;
    private float orignialAgroRange = 10.0f;
    private float orignialAttackCooldown = 0.5f;
    private float orignialMiningRate = 3.0f;
    private float orignialMineTime = 5.0f;
    private float orignialMineMaxInv = 3.0f;
    private float orignialRepairTime = 5.0f;
    private float orignialRepairAmount = 1.0f;


    public void iWasHit(GameObject attacker)
    {
        //If I was hit then target agressor and we are on attack stance
        if (attackState == AttackState.ATTACK)
        {
            target.Reset();
            target = new TargetObject(attacker);
        }
    }
    public void updateWorkerType(DroneMode newType)
    {
        droneMode = newType;
    }

    public aiDebug returnDebug()
    {
        return new aiDebug(target, stuck, idle, attackState);
    }
    
    public bool isIdle()
    {
        return idle;
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

            //Do not go somewhere we shouldn't according to our flags

            //We can mine or attach or bomb and the target is a resource
            if ((canAttachTo || canMine || bomber) && target.objID.objID == ObjectID.OBJECTID.RESOURCE)
            {
                //Find a position
                target.tarObjectAdjustPos = GetAdjustedPos();
            }
            //We can attack or repair and the target is a building that we own
            else if ((canAttachTo || canRepair) && target.objID.objID == ObjectID.OBJECTID.BUILDING && target.objID.ownerPlayerID == objID.ownerPlayerID)
            {
                //Find a position
                target.tarObjectAdjustPos = GetAdjustedPos();
            }
            //We can fight or attack or we are a bomber and the target is a building or unit the enemy owns
            else if ((canFight || canAttachTo || bomber) && (target.objID.objID == ObjectID.OBJECTID.BUILDING || target.objID.objID == ObjectID.OBJECTID.UNIT) && target.objID.ownerPlayerID != objID.ownerPlayerID)
            {
                //Do not boost a unit
                if (canAttachTo && target.objID.objID == ObjectID.OBJECTID.BUILDING)
                {
                    //Find a position
                    target.tarObjectAdjustPos = GetAdjustedPos();
                }
                else if (!canAttachTo)
                {
                    //Find a position
                    target.tarObjectAdjustPos = GetAdjustedPos();
                }
            }
            else
            {
                Debug.Log($"Invalid Request From UpdateTargetPos on Object [{gameObject.name}], check flags");
                target.Reset();
            }
        }
        else {
            //Go to pos
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
    void DealDamage()
    {
        if (attackTimer > attackCooldown)
        {
            if (!bomber)
            {
                StartCoroutine(FlashingMyLaserForThePlayerOwO());

                //Crits for fun
                int dice = Random.Range(1, 13);
                if (dice == 12)
                {
                    target.objID.health -= attackDamage * (Random.Range(1.5f, 2.5f));
                }
                else
                {
                    target.objID.health -= attackDamage;
                }

                if (target.tarObject.GetComponent<AIDroneController>() != null)
                {
                    target.tarObject.GetComponent<AIDroneController>().iWasHit(gameObject);
                }

                attackTimer = 0.0f;
            }
            //Mr bomb man
            else
            {
                //Get all objs within range needs a little more range in case it doesn't hit building
                Collider[] cols = Physics.OverlapSphere(transform.position, attackRange + 2.0f, interactLayer);

                //Damage all objs in range
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].gameObject.GetComponent<ObjectID>() != null) {
                        //If Building do full damage
                        ObjectID tmpObjID = cols[i].gameObject.GetComponent<ObjectID>();
                        if (tmpObjID.objID == ObjectID.OBJECTID.BUILDING)
                        {
                            cols[i].gameObject.GetComponent<ObjectID>().health -= attackDamage;
                        }
                        //If Unit do 1/5 damage
                        else if (tmpObjID.objID == ObjectID.OBJECTID.BUILDING)
                        {
                            cols[i].gameObject.GetComponent<ObjectID>().health -= attackDamage * 0.2f;
                        }
                        //If resource push and do 1/5 damage
                        else if (tmpObjID.objID == ObjectID.OBJECTID.RESOURCE)
                        {
                            cols[i].GetComponent<Rigidbody>().AddExplosionForce(attackDamage * 2.5f, transform.position, attackRange + 2.0f);
                            cols[i].gameObject.GetComponent<ObjectID>().health -= attackDamage * 0.2f;
                        }
                    }
                }

                //Explode
                Destroy(gameObject);

            }
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
                GM.UpdateResourceCount((int)objID.ownerPlayerID, currentInv);
                currentInv = 0;
            }
            TCRetOverride = false;
        }

    }

    void Repair()
    {
        Debug.Log("Repair() called");
        if (repairTimer >= repairTime)
        {
            StartCoroutine(FlashingMyLaserForThePlayerOwO());
            if ((target.objID.health + repairAmount) < target.objID.maxHealth)
            {
                target.objID.health += repairAmount;
            }
            else
            {
                target.objID.health = target.objID.maxHealth;
            }
            repairTimer = 0.0f;
        }
        //If repaired
        else if (target.objID.health == target.objID.maxHealth)
        {
            target.Reset();
        }
    }

    void Mine()
    {
        if (mineTimer >= mineTime)
        {
            StartCoroutine(FlashingMyLaserForThePlayerOwO());
            if (currentInv >= mineMaxInv)
            {
                //Go deposit inv at TC
                TCRetOverride = true;

            }
            else
            {
                //Mine some more
                target.objID.health -= attackDamage;
                currentInv += miningRate;
                mineTimer = 0.0f;
            }
        }
    }

    void Attach()
    {
        Debug.Log("Attach() Called");
        //Raycast and find closet point from our position then attach ourselves onto it and switch into asteriod mode, parent to asteriod also disable AI
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (target.tarObject.transform.position - transform.position), out hit, interactLayer))
        {
            transform.position = hit.point;
            agent.enabled = false;
            transform.parent = target.tarObject.transform;
            GetComponent<CapsuleCollider>().enabled = false;
            if (target.tarObject.GetComponent<Rigidbody>() == null)
            {
                target.tarObject.AddComponent<Rigidbody>();
            }
            asteriodBody = target.tarObject.GetComponent<Rigidbody>();
            asteriodBody.useGravity = false;
            asteriodBody.constraints = RigidbodyConstraints.FreezePositionY;
            asteriodBody.constraints = RigidbodyConstraints.FreezeRotationX;
            asteriodBody.constraints = RigidbodyConstraints.FreezeRotationZ;
            asteriodBody.angularDrag = 9999;
            asteriodOverride = true;
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
        if (target.objID.velo > 0.1f)
        {
            float time = Vector3.Distance(transform.position, target.tarObject.transform.position) / target.objID.velo;

            float distanceMoved = time * target.objID.velo;

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
        if (target.objID.velo > 0.1f)
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
        Debug.Log($"Attacking Object: {target.tarObject.name}");
        NavMeshPath path = new NavMeshPath();
        //Arrived at target object position
        if ((Vector3.Distance(transform.position, target.tarObjectAdjustPos) <= attackRange) || HasNeighbourStopped())
        {
            //If the target is within range
            if (Vector3.Distance(transform.position, target.tarObjectAdjustPos) <= attackRange)
            {
                //Stop we have reached target
                Stop();

                //If the target is a unit or building
                if (target.objID.objID == ObjectID.OBJECTID.UNIT || target.objID.objID == ObjectID.OBJECTID.BUILDING)
                {
                    //If it is our building and we can repair or attach
                    if (target.objID.ownerPlayerID == objID.ownerPlayerID)
                    {
                        if (canRepair)
                        {
                            Repair();
                        }
                        else if (canAttachTo)
                        {
                            Attach();
                        }
                        else
                        {
                            target.Reset();
                        }
                    }
                    //Is this a enemy building or unit?
                    else if ((target.objID.ownerPlayerID != objID.ownerPlayerID && target.objID.objID == ObjectID.OBJECTID.BUILDING) || (target.objID.objID == ObjectID.OBJECTID.UNIT)){
                        if (canFight)
                        {
                            DealDamage();
                        }
                        else if (canAttachTo)
                        {
                            Attach();
                        }
                        else
                        {
                            target.Reset();
                        }
                    }
                }
                //Is this a resource?
                else if (target.objID.objID == ObjectID.OBJECTID.RESOURCE)
                {
                    if (canMine)
                    {
                        Mine();
                    }
                    else if (canAttachTo)
                    {
                        Attach();
                    }
                    else if (bomber)
                    {
                        DealDamage();
                    }
                    else
                    {
                        target.Reset();
                    }
                }
                else
                {
                    Debug.LogWarning("Unspecified action for target {" + target.objID.objID + "}");
                }
            }
            //Neighbour Stopped
            else
            {
                //We are not in range goto enemy overriding neighbour
                if ((Vector3.Distance(transform.position, target.tarObject.transform.position) > attackRange))
                {
                    target.tarObjectAdjustPos = GetAdjustedPos();
                    agent.CalculatePath(target.tarObjectAdjustPos, path);
                    agent.SetPath(path);
                    Resume();
                }
                //Stop
                else
                {
                    Stop();
                }
            }
        }


        //If we haven't destroyed our gameobject above
        if (target.tarObject != null)
        {
            //Get to the enemy
            if (Vector3.Distance(transform.position, target.tarObject.transform.position) > attackRange)
            {
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

        //Override TC flag
        TCRetOverride = false;

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

        cols = Physics.OverlapSphere(transform.position, agroRange, interactLayer);

        List<GameObject> foundResources = new List<GameObject>();

        for (int i = 0; i < cols.Length; i++)
        {
            //Ignore if is not within attackRange and we are standing ground
            if ((Vector3.Distance(cols[i].transform.position, transform.position) <= attackRange && attackState == AttackState.STANDGROUND) || attackState == AttackState.ATTACK) {
                //Ignore our own objects
                if (cols[i].gameObject.GetComponent<ObjectID>() != null)
                {
                    if (cols[i].gameObject.GetComponent<ObjectID>().ownerPlayerID != objID.ownerPlayerID)
                    {
                        //Add if is a resource and we in mine mode and we can mine
                        if (canMine && cols[i].gameObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.RESOURCE)
                        {
                            foundResources.Add(cols[i].gameObject);
                        }
                        //If we can attack or bomb and is building or unit
                        else if ((bomber || canFight) && (cols[i].gameObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.BUILDING || cols[i].gameObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.UNIT))
                        {

                            //Make a new Target
                            target = new TargetObject(cols[i].gameObject);

                            //Get a good position near to the target in a arc realitve to our position
                            target.tarObjectAdjustPos = GetAdjustedPos();

                            return true;
                        }
                    }
                }
            }
        }

        //Fallback onto resources if no unity were found
        if (foundResources.Count > 0)
        {
            Debug.Log("Hit fallback");

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

    void UpdateType()
    {
        //Reset All Values
        attackRange = orignialAttackRange;
        attackDamage = orignialAttackDamage;
        agroRange = orignialAgroRange;
        attackCooldown = orignialAttackCooldown;
        miningRate = orignialMiningRate;
        mineTime = orignialMineTime;
        mineMaxInv = orignialMineMaxInv;
        repairTime = orignialRepairTime;
        repairAmount = orignialRepairAmount;
        objID.ResetHealthToOriginal();

        if (droneMode == DroneMode.WORKER)
        {
            canRepair = true;
            canMine = true;
            canFight = true;
            bomber = false;
            canAttachTo = false;
        }
        else if (droneMode == DroneMode.MINER)
        {
            canRepair = true;
            canMine = true;
            canFight = false;
            bomber = false;
            canAttachTo = false;

            //Give a 250% boost to mining rate and inv
            miningRate *= 2.5f;
            mineMaxInv = Mathf.Round(2.5f * mineMaxInv);

        }
        else if (droneMode == DroneMode.FIGHTER)
        {
            canRepair = false;
            canMine = false;
            canFight = true;
            bomber = false;
            canAttachTo = false;

            //Give a 250% boost to attackDamage and a 15% boost to range
            agroRange *= 2.5f;
            attackDamage *= 2.5f;

        }
        else if (droneMode == DroneMode.BOOSTER)
        {
            canRepair = false;
            canMine = false;
            canFight = false;
            bomber = false;
            canAttachTo = true;
        }
        else if (droneMode == DroneMode.BOMBER)
        {
            canRepair = true;
            canMine = false;
            canFight = true;
            bomber = true;
            canAttachTo = false;

            //Make glass cannon
            attackDamage *= 10.0f;
            objID.ModifyMaxHealth(objID.maxHealth * 0.1f);
        }
        else
        {
            Debug.LogWarning($"Cannot assign correct values as drone mode unknown: {droneMode}");
        }
    }

    void Start()
    {

        //Setup defaults
        orignialAttackRange = attackRange;
        orignialAttackDamage = attackDamage;
        orignialAgroRange = agroRange;
        orignialAttackCooldown = attackCooldown;
        orignialMiningRate = miningRate;
        orignialMineTime = mineTime;
        orignialMineMaxInv = mineMaxInv;
        orignialRepairTime = repairTime;
        orignialRepairAmount = repairAmount;

        //Setup Components
        agent = GetComponent<NavMeshAgent>();
        objID = GetComponent<ObjectID>();
        objID.objID = ObjectID.OBJECTID.UNIT;
        interactLayer = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().unitInteractLayers;
        target = new TargetObject(Vector3.zero);
        FindTC();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }


    void FixedUpdate()
    {

        if (!aiLock)
        {
            if (!asteriodOverride)
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
                idle = (agent.velocity.magnitude < 0.1f);
                if (idle)
                {
                    idleTimer += Time.unscaledDeltaTime;
                }
                else
                {
                    idleTimer = 0.0f;
                }

                //Stuck logic (path pending + no movement)
                stuck = (idle && (agent.pathPending || agent.pathStatus == NavMeshPathStatus.PathPartial));

                //We are stupid stuck
                if (idle && idleTimer > maxstuckTime)
                {
                    stuck = true;
                }


                if (stuck)
                {
                    if (idleTimer > maxstuckTime)
                    {
                        idleTimer = 0.0f;
                        target.Reset();
                        Stop();
                    }
                }

                //Update Laser Beam
                if (lr != null)
                {
                    lr.SetPosition(0, laserEmitterPositions[currentLaserEmitter].transform.position);
                    if (target.hasTargetObj())
                    {
                        lr.SetPosition(1, target.tarObject.transform.position);
                    }
                }

                //Check and fix things
                target.Sanity();
                UpdateType();


                if (TCRetOverride)
                {
                    TCRetOverrideBehaviour();
                }
                else
                {
                    if (idle && !target.hasTarget())
                    {
                        //If we have rocks and we don't have anywhere to go
                        if (idle && currentInv > 0)
                        {
                            TCRetOverride = true;
                        }
                        else
                        {
                            IdleAttackLogic();
                        }
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
            //We are in asteriod mode
            else
            {
                Debug.Log("Ateriod Mode");
            }
        }
    }


    IEnumerator FlashingMyLaserForThePlayerOwO()
    {
        //Turn Laser On
        if (lr != null)
        {
            currentLaserEmitter = Random.Range(0, laserEmitterPositions.Count);
            lr.enabled = true;
        }

        yield return new WaitForSeconds(laserSustainTime);
        
        //Turn Laser Off
        lr.enabled = false;
    }

}
