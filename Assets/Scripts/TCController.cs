using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCController : MonoBehaviour
{
    public GameObject unitTemplate;
    public GameObject carWashMiner;
    public GameObject carWashBoost;
    public GameObject carWashFighter;
    public float baseCost = 100;
    public float mineCost = 5000;
    public float attackCost = 100000;
    public float boostCost = 150000;
    public float housecost = 20000;
    public float escapeCost = 999999;

    public GameObject gamewinUI;
    public GameObject gameLossUI;

    private bool registered = false;
    private ObjectID objID;
    private GameManager GM;
    private PlayerController playerCtrl;
    public List<GameObject> playerunit = new List<GameObject>();
    private int unitCount = 0;
    private bool Modifylock = false;
    private bool uionce = true;

    public enum STORE
    {
        BASE,
        MINECW,
        ATTACKCW,
        BOOSTCW,
        HOUSE,
        ESCAPE,
    };

    public void SpawnUnit(TCController.STORE tospawn)
    {
        SpawnUnit(tospawn, false, null, AIDroneController.DroneMode.WORKER);
    }

    public void SpawnUnit(TCController.STORE tospawn, bool amAI)
    {
        SpawnUnit(tospawn, amAI, null, AIDroneController.DroneMode.WORKER);
    }
    public void SpawnUnit(TCController.STORE tospawn, bool amAI, AIDroneController.DroneMode dronemode)
    {
        SpawnUnit(TCController.STORE.BASE, true, null, dronemode);
    }

    public void SpawnUnit(TCController.STORE tospawn, bool amAI, AIBehaviour.outpostBuilding aiBuilding)
    {
        SpawnUnit(TCController.STORE.BASE, true, aiBuilding, AIDroneController.DroneMode.WORKER);
    }

    public void SpawnUnit(TCController.STORE tospawn, bool amAI, AIBehaviour.outpostBuilding aiBuilding, AIDroneController.DroneMode droneMode)
    {
        if (tospawn == STORE.BASE)
        {
            if (!Modifylock && (GM.GetResouceCount((int)objID.ownerPlayerID) >= baseCost) && (GM.GetUnitCount((int)objID.ownerPlayerID) < GM.GetUnitCountMax((int)objID.ownerPlayerID)))
            {
                Debug.Log("spawn base");
                GM.UpdateResourceCount((int)objID.ownerPlayerID, -baseCost);
                GameObject spawnedObj = Instantiate(unitTemplate, (transform.position + (new Vector3(Random.Range(-1.0f,1.0f),0.0f, Random.Range(-1.0f, 1.0f)) * 5.0f)), Quaternion.identity);
                spawnedObj.GetComponent<ObjectID>().ownerPlayerID = objID.ownerPlayerID;
                if (!amAI)
                {
                    playerunit.Add(spawnedObj);
                }
                else
                {
                    spawnedObj.GetComponent<AIDroneController>().droneMode = droneMode;
                }
            }
        }
        else if (tospawn == STORE.MINECW)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >= mineCost)
            { 
                if (!amAI)
                {
                    Debug.Log("spawn mine");
                    GM.UpdateResourceCount((int)objID.ownerPlayerID, -mineCost);

                    playerCtrl.lastSelectedBuildingToBuild = carWashMiner;
                }
                else
                {
                    GM.UpdateResourceCount((int)objID.ownerPlayerID, -mineCost);
                    Instantiate(carWashMiner, aiBuilding.lastSeenPosition, Quaternion.identity);
                }
            }
        }
        else if (tospawn == STORE.ATTACKCW)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >= attackCost)
            {
                if (!amAI)
                {
                    Debug.Log("spawn attack");
                    GM.UpdateResourceCount((int)objID.ownerPlayerID, -attackCost);


                    playerCtrl.lastSelectedBuildingToBuild = carWashFighter;
                }
                else
                {
                    GM.UpdateResourceCount((int)objID.ownerPlayerID, -attackCost);
                    Instantiate(carWashFighter, aiBuilding.lastSeenPosition, Quaternion.identity);
                }
            }
        }
        else if (tospawn == STORE.BOOSTCW)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >= boostCost)
            {
                if (!amAI)
                {
                    Debug.Log("spawn boost");
                    GM.UpdateResourceCount((int)objID.ownerPlayerID, -boostCost);


                    playerCtrl.lastSelectedBuildingToBuild = carWashBoost;
                }
                else
                {
                    GM.UpdateResourceCount((int)objID.ownerPlayerID, -boostCost);
                    Instantiate(carWashBoost, aiBuilding.lastSeenPosition, Quaternion.identity);
                }
            }
        }
        else if (tospawn == STORE.HOUSE)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >=  housecost)
            {
                if (!amAI)
                {
                    Debug.Log("spawn house");
                    GM.UpdateResourceCount((int)objID.ownerPlayerID, -housecost);

                    GM.setUnitCountMax((int)objID.ownerPlayerID, GM.GetUnitCountMax((int)objID.ownerPlayerID) + 10);
                    //playerCtrl.lastSelectedBuildingToBuild = carWashBoost;
                }
                else
                {

                }
            }
        }
        else if (tospawn == STORE.ESCAPE)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >= escapeCost)
            {
                GM.UpdateResourceCount((int)objID.ownerPlayerID, -escapeCost);
                if (objID.ownerPlayerID == ObjectID.PlayerID.PLAYER)
                {
                    gamewinUI.SetActive(true);
                    Debug.Log("escape");


                }

                this.gameObject.GetComponent<Rigidbody>().AddForce(-(GameObject.Find("Blackhole").gameObject.transform.position - transform.position).normalized * 10.0f, ForceMode.Impulse);

            }
        }

    }
    void Start()
    {
        objID = GetComponent<ObjectID>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GetComponent<Rigidbody>().Sleep();
        playerCtrl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>();


        if (objID.ownerPlayerID == ObjectID.PlayerID.PLAYER)
        {

            gamewinUI = GameObject.Find("Escaped");
            gameLossUI = GameObject.Find("GameOver");

            gamewinUI.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            gameLossUI.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

            gamewinUI.SetActive(false);
            gameLossUI.SetActive(false);
        }


    }

    void FixedUpdate()
    {

        if (!registered)
        {
            GetComponent<Rigidbody>().Sleep();
        }

        if (!registered && objID.ownerPlayerID != ObjectID.PlayerID.UNASSIGNED)
        {
            if (objID.ownerPlayerID != ObjectID.PlayerID.PLAYER)
            {
                GM.regTC(true, gameObject);
            }
            else
            {
                GM.regTC(false, gameObject);
            }
            registered = true;
        }
        else
        {
            GM.setUnitCount((int)objID.ownerPlayerID, unitCount);
        }

        if (uionce == true)
        {
            if (objID.ownerPlayerID == ObjectID.PlayerID.PLAYER)
            {
                if (objID.health < (objID.maxHealth / 2))
                {
                    uionce = false;
                    GameObject.Find("HPbloom").GetComponent<MakeFlash>().flashing = true;
                    GameObject flasher = GameObject.Find("MakeFlash");
                    flasher.transform.GetChild(0).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(1).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(2).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(3).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(4).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(5).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(6).GetComponent<MakeFlash>().flashing = true;


                }
                else if (GameObject.Find("Blackhole").GetComponent<Blackhole>().timer > ((GameObject.Find("Blackhole").GetComponent<Blackhole>().twomintimer / 4) * 3))
                {
                    uionce = false;
                    GameObject.Find("HPbloom").GetComponent<MakeFlash>().flashing = true;
                    GameObject flasher = GameObject.Find("MakeFlash");

                    flasher.transform.GetChild(0).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(1).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(2).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(3).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(4).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(5).GetComponent<MakeFlash>().flashing = true;
                    flasher.transform.GetChild(6).GetComponent<MakeFlash>().flashing = true;
                }
            }
        }
 
    }

    void Update()
    {
        for (int i = 0; i < playerunit.Count; i++)
        {
            if (playerunit[i] == null)
            {
                playerunit.RemoveAt(i);
            }
        }
        unitCount = playerunit.Count;
        GM.setUnitCount((int)objID.ownerPlayerID, playerunit.Count);
    }

}
