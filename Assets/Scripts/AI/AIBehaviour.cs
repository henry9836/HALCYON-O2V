using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{

    public class scoutedObject
    {
        public scoutedObject(GameObject _obj)
        {
            if (_obj != null)
            {
                obj = _obj;
                positionSpotted = obj.transform.position;
                objID = obj.GetComponent<ObjectID>();
                ownerID = objID.ownerPlayerID;
                objType = objID.objID;

                if (obj.GetComponent<TCController>())
                {
                    isTC = true;
                }
            }
        }

        public GameObject obj;
        public ObjectID objID;
        public bool isTC = false;
        public float distanceFromUs;
        public Vector3 positionSpotted;
        public ObjectID.OBJECTID objType;
        public ObjectID.PlayerID ownerID;
    };

    public class aiObject
    {
        public aiObject(GameObject _obj)
        {
            obj = _obj;
            objID = obj.GetComponent<ObjectID>();
            ownerID = objID.ownerPlayerID;
            aiCtrl = obj.GetComponent<AIDroneController>();
        }

        public GameObject obj;
        public ObjectID objID;
        public bool isTC = false;
        public float distanceFromUs;
        public ObjectID.PlayerID ownerID;
        public AIDroneController aiCtrl;
    };

    public int playerID = -1;
    public Vector2 profitCheckRandomRange = new Vector2(5.0f, 20.0f);

    //Privates
    public List<float> balanceHistory = new List<float>() { Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity };
    private List<aiObject> units = new List<aiObject>();
    private List<aiObject> idleUnits = new List<aiObject>();
    private List<aiObject> enemyUnits = new List<aiObject>();
    private List<scoutedObject> enemyBuilds = new List<scoutedObject>();
    private List<scoutedObject> resources = new List<scoutedObject>();
    private GameManager GM;
    private ObjectID objID;
    private TCController TC;
    private GameObject scoutUnit;
    private GameObject blackHole;
    private GameObject ground;
    public GameObject closestKnownResource = null;
    public GameObject closestKnownEnemyBuilding = null;
    public GameObject closestKnownEnemyTC = null;
    public aiObject closestKnownEnemyUnit = null;
    public float profitCheckTimer = 0.0f;
    public float profitCheckThreshold = 7.0f;
    public float lastBalanceAvg = Mathf.Infinity;
    private float knowledgeTimer = 0.0f;
    private float knowledgeThreshold = 5.0f;
    private float acceptableAsteriodDistance = 100.0f;
    private bool AIStepLock = false;
    private float baseCost = 100;
    private float mineCost = 5000;
    private float attackCost = 100000;
    private float boostCost = 150000;
    private float escapeCost = 999999;

    // Start is called before the first frame update
    void Start()
    {
        objID = GetComponent<ObjectID>();
        if (objID.ownerPlayerID == ObjectID.PlayerID.UNASSIGNED)
        { 
            playerID = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RequestID((int)ObjectID.PlayerID.AI_1);
            objID.ownerPlayerID = (ObjectID.PlayerID)playerID;
        }
        else
        {
            playerID = (int)objID.ownerPlayerID;
        }

        //Set up timers
        profitCheckThreshold = Random.Range(profitCheckRandomRange.x, profitCheckRandomRange.y);

        //Find References
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        TC = GetComponent<TCController>();
        blackHole = GameObject.FindGameObjectWithTag("Blackhole");
        ground = GameObject.FindGameObjectWithTag("Ground");
        acceptableAsteriodDistance = Vector3.Distance(ground.transform.position, GameObject.FindGameObjectWithTag("Henry'sStupidCube").transform.position);

        baseCost = TC.baseCost;
        mineCost = TC.mineCost;
        attackCost = TC.attackCost;
        boostCost = TC.boostCost;
        escapeCost = TC.escapeCost;
}

    void TickTickTickTickTickTickTickTickTickTickTickTickTickTick()
    {
        profitCheckTimer += Time.unscaledDeltaTime;
    }

    void Escape()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //TICK TICK TICK TICK TICK TICK TICK TICK TICK TICK TICK 
        TickTickTickTickTickTickTickTickTickTickTickTickTickTick();
        //IF ESCAPE IS THING THEN DO THE ESCAPE THING
        if (escapeCost <= GM.GetResouceCount(playerID))
        {
            Escape();
        }

        //OTHERWISE DO THE OTHER THINGS
        else if (!AIStepLock)
        {
            StartCoroutine(AIStep());
        }
    }
    
    IEnumerator AIStep()
    {
        //Lock
        AIStepLock = true;

        /*
         * 
         * CULL NULLS
         * 
         */

        for (int i = 0; i < units.Count; i++)
        {
            //Is the obj null?
            if (units[i] == null)
            {
                //Remove obj if null
                units.RemoveAt(i);
            }
            yield return null;
        }

        /*
         * 
         * FIND OBJECTS
         * 
         */


        GameObject[] foundResources = GameObject.FindGameObjectsWithTag("Resource");
        GameObject[] foundWorldUnits = GameObject.FindGameObjectsWithTag("Unit");
        GameObject[] foundBuildingsTCs = GameObject.FindGameObjectsWithTag("TC");
        GameObject[] foundBuildingsCWs = GameObject.FindGameObjectsWithTag("CarWash");

        /*
         * 
         * POPULATE LISTS
         * 
         */

        //Clear lists
        idleUnits.Clear();
        resources.Clear();
        enemyUnits.Clear();
        enemyBuilds.Clear();


        float tmpDistance = Mathf.Infinity;

        for (int i = 0; i < units.Count; i++)
        {
            //If one of our units is idle add to idle list
            if (units[i].aiCtrl.isIdle())
            {
                idleUnits.Add(units[i]);
            }
            yield return null;
        }

        tmpDistance = Mathf.Infinity;

        for (int i = 0; i < foundBuildingsTCs.Length; i++)
        {
            //Filter out friendlies
            if (foundBuildingsTCs[i].GetComponent<ObjectID>().ownerPlayerID != (ObjectID.PlayerID)playerID)
            {
                enemyBuilds.Add(new scoutedObject(foundBuildingsTCs[i]));
                if (Vector3.Distance(transform.position, enemyBuilds[enemyBuilds.Count - 1].obj.transform.position) < tmpDistance)
                {
                    closestKnownEnemyTC = enemyBuilds[enemyBuilds.Count - 1].obj;
                    tmpDistance = Vector3.Distance(transform.position, enemyBuilds[enemyBuilds.Count - 1].obj.transform.position);
                }
            }
            yield return null;
        }

        tmpDistance = Mathf.Infinity;

        for (int i = 0; i < foundBuildingsCWs.Length; i++)
        {
            //Filter out friendlies
            if (foundBuildingsCWs[i].GetComponent<ObjectID>().ownerPlayerID != (ObjectID.PlayerID)playerID)
            {
                enemyBuilds.Add(new scoutedObject(foundBuildingsCWs[i]));
                if (Vector3.Distance(transform.position, enemyBuilds[enemyBuilds.Count - 1].obj.transform.position) < tmpDistance)
                {
                    closestKnownEnemyBuilding = enemyBuilds[enemyBuilds.Count - 1].obj;
                    tmpDistance = Vector3.Distance(transform.position, enemyBuilds[enemyBuilds.Count - 1].obj.transform.position);
                }
            }
            yield return null;
        }

        tmpDistance = Mathf.Infinity;

        for (int i = 0; i < foundWorldUnits.Length; i++)
        {
            //Filter out friendlies
            if (foundWorldUnits[i].GetComponent<ObjectID>().ownerPlayerID != (ObjectID.PlayerID)playerID)
            {
                enemyUnits.Add(new aiObject(foundWorldUnits[i]));
                if (Vector3.Distance(transform.position, enemyUnits[enemyUnits.Count - 1].obj.transform.position) < tmpDistance)
                {
                    closestKnownEnemyUnit = enemyUnits[enemyUnits.Count - 1];
                    tmpDistance = Vector3.Distance(transform.position, enemyUnits[enemyUnits.Count - 1].obj.transform.position);
                }
            }
            yield return null;
        }

        tmpDistance = Mathf.Infinity;

        for (int i = 0; i < foundResources.Length; i++)
        {
            //If not inside the game world enough
            Debug.Log($"Our distance to resource is: {Vector3.Distance(foundResources[i].transform.position, transform.position)} and we only allow within {acceptableAsteriodDistance}");
            if (Vector3.Distance(foundResources[i].transform.position, transform.position) < acceptableAsteriodDistance)
            {
                resources.Add(new scoutedObject(foundResources[i]));

                if (resources[resources.Count - 1].obj != null)
                {
                    if (Vector3.Distance(transform.position, resources[resources.Count - 1].obj.transform.position) < tmpDistance)
                    {
                        closestKnownResource = resources[resources.Count - 1].obj;
                        tmpDistance = Vector3.Distance(transform.position, resources[resources.Count - 1].obj.transform.position);
                    }
                }
            }
            yield return null;
        }

        /*
         * 
         * SAFETY IS NUMBER SIX PRIORITY
         * 
         */

        //Check if too close to blackhole if so try and come back towards TC out of blackhole range
        for (int i = 0; i < units.Count; i++)
        {
            //If we are too close to blackhole
            if (Vector3.Distance(transform.position, blackHole.transform.position) < blackHole.transform.localScale.x + 2.0f)
            {
                //Move away
                Vector3 dir = (transform.position - blackHole.transform.position).normalized;
                Vector3 escapePos = transform.position + (dir * 10.0f);
                units[i].aiCtrl.UpdateTargetPos(escapePos, null);
            }

            yield return null;
        }

        /*
         * 
         * PROFIT CHECK
         * 
         */

        bool madeProfit = false;
        bool validProfitCheck = false;

        if (profitCheckTimer > profitCheckThreshold)
        {
            //Reset timer
            profitCheckThreshold = Random.Range(profitCheckRandomRange.x, profitCheckRandomRange.y);
            profitCheckTimer = 0.0f;

            //Do we have enough history to get avg?
            if (balanceHistory.Count >= 5)
            {
                float avg = 0.0f;

                //Get average
                for (int i = 0; i < balanceHistory.Count; i++)
                {
                    avg += balanceHistory[i];

                    yield return null;
                }
                avg /= balanceHistory.Count;

                //Is the avg not higher than our last average?
                if (avg < lastBalanceAvg)
                {
                    madeProfit = false;
                    validProfitCheck = true;
                }
                else
                {
                    madeProfit = true;
                    validProfitCheck = true;
                }

                //Update last balance and clear list
                lastBalanceAvg = avg;
                balanceHistory.Clear();

            }
            //Add onto history
            else
            {
                balanceHistory.Add(GM.GetResouceCount(playerID));
            }
        }




        /*
         * 
         * MINERS
         * 
         */

       
        //If we didn't make a profit
        if (!madeProfit && validProfitCheck)
        {
            int amountofUnitsAffected = 0;

            //Do we have idle miners
            for (int i = 0; i < idleUnits.Count; i++)
            {
                if ((idleUnits[i].aiCtrl.droneMode == AIDroneController.DroneMode.MINER) || (idleUnits[i].aiCtrl.droneMode == AIDroneController.DroneMode.WORKER))
                {
                    //Target resource until we have at least five
                    idleUnits[i].aiCtrl.UpdateTargetPos(Vector3.zero, closestKnownResource);
                    amountofUnitsAffected++;
                }
                if (amountofUnitsAffected >= 5)
                {
                    break;
                }

                yield return null;
            }

            //Make more miners if we don't have idle miners
            if (amountofUnitsAffected < 5)
            {
                //build more units
                for (int i = amountofUnitsAffected; i < 5; i++)
                {
                    TC.SpawnUnit(TCController.STORE.BASE);
                }
            }
        }

        /*
         * 
         * Does anyone have bad rep
         * 
         */


        /*
         * 
         * Is Unit Count OK
         * 
         */

        /*
         * 
         * Is base missing Things
         * 
         */

        //Unlock
        AIStepLock = false;
        yield return null;
    }



}
