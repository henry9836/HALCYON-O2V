using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{

    public class scoutedObject
    {
        public scoutedObject(GameObject _obj)
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

    public List<float> balanceHistory = new List<float>();
    private List<aiObject> units = new List<aiObject>();
    private List<aiObject> idleUnits = new List<aiObject>();
    private List<aiObject> enemyUnits = new List<aiObject>();
    private List<scoutedObject> enemyBuilds = new List<scoutedObject>();
    private List<scoutedObject> resources = new List<scoutedObject>();
    private GameManager GM;
    private ObjectID objID;
    public float profitCheckTimer = 0.0f;
    public float profitCheckThreshold = 7.0f;
    public float lastBalanceAvg = Mathf.Infinity;
    private float knowledgeTimer = 0.0f;
    private float knowledgeThreshold = 5.0f;
    private float currentbalance = 0.0f;
    private bool AIStepLock = false;
    private scoutedObject closestKnownResource = null;
    private scoutedObject closestKnownEnemyUnit = null;
    private scoutedObject closestKnownEnemyBuilding = null;
    private scoutedObject closestKnownEnemyTC = null;
    private TCController TC;
    private GameObject scoutUnit;

    void profitCheck()
    {
        profitCheckTimer += Time.unscaledDeltaTime;

        //Debug.Log($"{profitCheckTimer}/{profitCheckThreshold}");

        if (profitCheckTimer >= profitCheckThreshold)
        {

            

        }
    }

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!AIStepLock)
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

        for (int i = 0; i < units.Count; i++)
        {
            //If one of our units is idle add to idle list
            if (units[i].aiCtrl.isIdle())
            {
                idleUnits.Add(units[i]);
            }
        }

        for (int i = 0; i < foundBuildingsTCs.Length; i++)
        {
            //Filter out friendlies
            if (foundBuildingsTCs[i].GetComponent<ObjectID>().ownerPlayerID != (ObjectID.PlayerID)playerID)
            {
                enemyBuilds.Add(new scoutedObject(foundBuildingsTCs[i]));
            }
        }

        for (int i = 0; i < foundBuildingsCWs.Length; i++)
        {
            //Filter out friendlies
            if (foundBuildingsCWs[i].GetComponent<ObjectID>().ownerPlayerID != (ObjectID.PlayerID)playerID)
            {
                enemyBuilds.Add(new scoutedObject(foundBuildingsCWs[i]));
            }
        }

        for (int i = 0; i < foundWorldUnits.Length; i++)
        {
            //Filter out friendlies
            if (foundWorldUnits[i].GetComponent<ObjectID>().ownerPlayerID != (ObjectID.PlayerID)playerID)
            {
                enemyUnits.Add(new aiObject(foundWorldUnits[i]));
            }
        }

        for (int i = 0; i < foundResources.Length; i++)
        {
            resources.Add(new scoutedObject(foundResources[i]));
        }

        /*
         * 
         * PROFIT CHECK
         * 
         */

        bool madeProfit = false;
        currentbalance = GM.GetResouceCount(playerID);

        profitCheckTimer += Time.unscaledDeltaTime;

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
                }
                avg /= balanceHistory.Count;

                //Is the avg not higher than our last average?
                if (avg < lastBalanceAvg)
                {
                    madeProfit = false;
                }
                else
                {
                    madeProfit = true;
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

        //madeProfit do more work below me

        //Unlock
        AIStepLock = false;
        yield return null;
    }



}
