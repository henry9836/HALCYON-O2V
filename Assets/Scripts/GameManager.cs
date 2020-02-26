using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public class Bank
    {
        public int maxUnit = 10;
        public int currUnit = 0;
        public int ID;
        public float resourceCount;



        public Bank(int _ID, float _startingResource)
        {
            ID = _ID;
            resourceCount = _startingResource;
        }

    }

    enum EnemyDifficulty { 
        DUMMY,
        EASY,
        MED,
        HARD
    };

    int tcCount = 0;
    public GameObject TCTemplate;
    public GameObject minerCW;
    public GameObject fighterCW;
    public GameObject turret;
    public float defaultResources = 50.0f;
    public List<Bank> banks = new List<Bank>();
    public List<GameObject> playerTCs = new List<GameObject>();
    public List<GameObject> aiTCs = new List<GameObject>();

    private GameObject HSC;
    private float validDistanceInsideWorld;
    private LayerMask layerMask;


    public void regTC(bool amAI, GameObject caller)
    {
        if (amAI)
        {
            aiTCs.Add(caller);
        }
        else
        {
            playerTCs.Add(caller);
        }
        tcCount++;
    }

    bool DoesIDExist(int ID)
    {
        for (int i = 0; i < banks.Count; i++)
        {
            if (ID == banks[i].ID)
            {
                return true;
            }
        }

        return false;
    }

    public int RequestID(int reqID)
    {

        if (!DoesIDExist(reqID))
        {
            banks.Add(new Bank(reqID, defaultResources));
            return reqID;
        }
        else
        {
            while (true)
            {
                reqID++;
                if (!DoesIDExist(reqID))
                {
                    banks.Add(new Bank(reqID, defaultResources));
                    return reqID;
                }
            }
        }
    }

    public float GetResouceCount(int ID)
    {
        for (int i = 0; i < banks.Count; i++)
        {
            if (banks[i].ID == ID)
            {
                return banks[i].resourceCount;
            }
        }

        return 0.0f;
    }

    public int GetUnitCount(int ID)
    {
        for (int i = 0; i < banks.Count; i++)
        {
            if (banks[i].ID == ID)
            {
                return (banks[i].currUnit);
            }
        }
        return -999;
    }

    public int GetUnitCountMax(int ID)
    {
        for (int i = 0; i < banks.Count; i++)
        {
            if (banks[i].ID == ID)
            {
                return (banks[i].maxUnit);
            }
        }
        return -999;
    }

    public void setUnitCount(int ID, int set)
    {
        for (int i = 0; i < banks.Count; i++)
        {
            if (banks[i].ID == ID)
            {
                banks[i].currUnit = set;
                break;
            }
        }
    }

    public void setUnitCountMax(int ID, int set)
    {
        for (int i = 0; i < banks.Count; i++)
        {
            if (banks[i].ID == ID)
            {
                banks[i].maxUnit = set;
                break;
            }
        }
    }


    public void UpdateResourceCount(int ID, float change)
    {
        for (int i = 0; i < banks.Count; i++)
        {
            if (banks[i].ID == ID)
            {
                banks[i].resourceCount += change;
                break;
            }
        }
    }

    void CheckListsForNulls()
    {
        for (int i = 0; i < aiTCs.Count; i++)
        {
            if (aiTCs[i].gameObject == null)
            {
                aiTCs.RemoveAt(i);
            }
        }

        for (int i = 0; i < playerTCs.Count; i++)
        {
            if (playerTCs[i].gameObject == null)
            {
                playerTCs.RemoveAt(i);
            }
        }
    }
    private void Start()
    {
        layerMask = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().unitInteractLayers;
        HSC = GameObject.FindGameObjectWithTag("Henry'sStupidCube");
        validDistanceInsideWorld = Vector3.Distance(GameObject.FindGameObjectWithTag("Ground").transform.position, HSC.transform.position);
        StartCoroutine(SpawnTCs());
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

            //Once tcs are registered
            if (tcCount >= 2)
        {
            //Check who is alive
            //Player lost
            if (playerTCs.Count <= 0)
            {
                Debug.Log("AI WINS");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            //AI lost
            else if (aiTCs.Count <= 0)
            {
                Debug.Log("PLAYER WINS");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        CheckListsForNulls();
    }

    IEnumerator SpawnOutpostBuidling(GameObject targetTC, GameObject building)
    {
        bool foundValidSpawnPosition = false;
        float spawnRange = 20.0f;

        while (!foundValidSpawnPosition)
        {
            Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(transform.position.x - spawnRange, transform.position.x + spawnRange), transform.position.y, UnityEngine.Random.Range(transform.position.z - spawnRange, transform.position.z + spawnRange));

            //Get radius of object
            float checkRadius = building.transform.localScale.x / 2;

            //Check if colliding with anything and distance

            //If I hit then it is not valid spawn
            if (Physics.CheckSphere(spawnPos, checkRadius, layerMask))
            {
                foundValidSpawnPosition = false;
            }
            //Check distance
            else
            {
                //True if within distance
                foundValidSpawnPosition = (Vector3.Distance(spawnPos, GameObject.FindGameObjectWithTag("Ground").transform.position) < validDistanceInsideWorld);
            }

            if (foundValidSpawnPosition)
            {
                //Spawn
                GameObject refer = Instantiate(building, spawnPos, Quaternion.identity);

                //Assign ownership
                refer.GetComponent<ObjectID>().ownerPlayerID = targetTC.GetComponent<ObjectID>().ownerPlayerID;
            }

            yield return null;
        }


        yield return null;
    }

    IEnumerator SpawnTCs()
    {
        GameObject[] spawnLocationsArray = GameObject.FindGameObjectsWithTag("SpawnPosition");

        List<Vector3> spawnLocations = new List<Vector3>();

        bool firstLoop = true;
        Vector3 spawnPos = Vector3.zero;

        for (int i = 0; i < spawnLocationsArray.Length; i++)
        {
            spawnLocations.Add(spawnLocationsArray[i].transform.position);
            yield return null;
        }

        while ((playerTCs.Count + aiTCs.Count) < MagicTraveller.TCCount)
        {
            bool spawned = false;
            //Spawn Player
            if (playerTCs.Count < MagicTraveller.PlayerCounter)
            {
                //Select a random point if first spawn
                if (firstLoop)
                {
                    int element = UnityEngine.Random.Range(0, spawnLocations.Count);
                    spawnPos = spawnLocations[element];
                    spawnLocations.RemoveAt(element);
                }
                else
                {
                    float longestDistance = 0.0f;
                    int elementLongest = 0;
                    //Find a point furthest from ais
                    for (int i = 0; i < spawnLocations.Count; i++)
                    {
                        for (int j = 0; j < aiTCs.Count; j++)
                        {
                            //If we found a location that is further than previous discovery
                            if (Vector3.Distance(spawnLocations[i], aiTCs[j].transform.position) > longestDistance)
                            {
                                elementLongest = i;
                                longestDistance = Vector3.Distance(spawnLocations[i], aiTCs[j].transform.position);
                            }
                        }
                    }
                    //Update spawnPos with longest location away from AI
                    spawnPos = spawnLocations[elementLongest];
                    spawnLocations.RemoveAt(elementLongest);
                }

                //Spawn TC
                GameObject TCref = Instantiate(TCTemplate, spawnPos, Quaternion.identity);
                TCref.GetComponent<ObjectID>().ownerPlayerID = ObjectID.PlayerID.PLAYER;
                playerTCs.Add(TCref);
                firstLoop = false;
                spawned = true;
            }
            //Spawn AI
            if (aiTCs.Count < MagicTraveller.AIEnemyCount)
            {
                //Select a random point if first spawn
                if (firstLoop)
                {
                    int element = UnityEngine.Random.Range(0, spawnLocations.Count);
                    spawnPos = spawnLocations[element];
                    spawnLocations.RemoveAt(element);
                }
                else
                {
                    float longestDistance = 0.0f;
                    int elementLongest = 0;
                    //Find a point furthest from ais
                    for (int i = 0; i < spawnLocations.Count; i++)
                    {
                        for (int j = 0; j < playerTCs.Count; j++)
                        {
                            //If we found a location that is further than previous discovery
                            if (Vector3.Distance(spawnLocations[i], playerTCs[j].transform.position) > longestDistance)
                            {
                                elementLongest = i;
                                longestDistance = Vector3.Distance(spawnLocations[i], playerTCs[j].transform.position);
                            }
                        }
                    }
                    //Update spawnPos with longest location away from AI
                    spawnPos = spawnLocations[elementLongest];
                    spawnLocations.RemoveAt(elementLongest);
                }

                //Spawn TC
                GameObject TCref = Instantiate(TCTemplate, spawnPos, Quaternion.identity);
                TCref.AddComponent<AIBehaviour>();
                aiTCs.Add(TCref);
                firstLoop = false;
                spawned = true;
            }
            if (!spawned)
            {
                break;
            }

            firstLoop = false;

            StartCoroutine(SpawnOutposts());

            yield return null;
        }


    }

    IEnumerator SpawnOutposts()
    {

        //Delay for IDs and spawns to settle
        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < aiTCs.Count; i++)
        {
            //Pick a difficilty
            EnemyDifficulty difficulty = (EnemyDifficulty)UnityEngine.Random.Range((int)EnemyDifficulty.EASY, (int)EnemyDifficulty.HARD+1);

            Debug.Log($"Creating an outpost with difficulty set to {difficulty}");

            //CWs
            if (difficulty >= EnemyDifficulty.MED)
            {
                StartCoroutine(SpawnOutpostBuidling(aiTCs[i], minerCW));
            }

            if (difficulty >= EnemyDifficulty.HARD)
            {
                StartCoroutine(SpawnOutpostBuidling(aiTCs[i], fighterCW));
            }

            //Turrets
            int turretSpawnAmount = 0;

            switch (difficulty)
            {
                case EnemyDifficulty.EASY:
                    {
                        turretSpawnAmount = 1;
                        break;
                    }
                case EnemyDifficulty.MED:
                    {
                        turretSpawnAmount = 2;
                        break;
                    }
                case EnemyDifficulty.HARD:
                    {
                        turretSpawnAmount = 4;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            for (int f = 0; f < turretSpawnAmount; f++)
            {
                StartCoroutine(SpawnOutpostBuidling(aiTCs[i], turret));
            }

            //Starting Resource

            float multipler = 1.0f;

            if (difficulty >= EnemyDifficulty.MED)
            {
                multipler = 1.5f;
            }

            else if (difficulty >= EnemyDifficulty.HARD)
            {
                multipler = 2.0f;
            }

            for (int a = 0; a < banks.Count; a++)
            {
                if (banks[a].ID == aiTCs[i].GetComponent<AIBehaviour>().playerID)
                {
                    banks[a].resourceCount *= multipler;
                }
            }

        }

        yield return null;
    }

}
