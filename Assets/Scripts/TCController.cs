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
    public float escapeCost = 999999;

    private bool registered = false;
    private ObjectID objID;
    private GameManager GM;
    private PlayerController playerCtrl;
    public List<GameObject> playerunit = new List<GameObject>();
    private int unitCount = 0;

    public enum STORE
    {
        BASE,
        MINECW,
        ATTACKCW,
        BOOSTCW,
        ESCAPE,
    };

    public GameObject SpawnUnit(TCController.STORE tospawn, bool amAI, AIBehaviour.outpostBuilding aiBuilding)
    {
        if (tospawn == STORE.BASE)
        {
            if ((GM.GetResouceCount((int)objID.ownerPlayerID) >= baseCost) && (GM.GetUnitCount((int)objID.ownerPlayerID) < GM.GetUnitCountMax((int)objID.ownerPlayerID)))
            {
                Debug.Log("spawn base");
                GM.UpdateResourceCount((int)objID.ownerPlayerID, -baseCost);
                GameObject spawnedObj = Instantiate(unitTemplate, (transform.position + (new Vector3(Random.Range(-1.0f,1.0f),0.0f, Random.Range(-1.0f, 1.0f)) * 5.0f)), Quaternion.identity);
                spawnedObj.GetComponent<ObjectID>().ownerPlayerID = objID.ownerPlayerID;
                playerunit.Add(spawnedObj);
                GM.setUnitCount((int)objID.ownerPlayerID, playerunit.Count);
                return spawnedObj;
            }
        }
        else if (tospawn == STORE.MINECW)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >= mineCost)
            { 
                if (!amAI)
                {
                    Debug.Log("spawn mince");

                    playerCtrl.lastSelectedBuildingToBuild = carWashMiner;
                }
                else
                {

                }
            }
        }
        else if (tospawn == STORE.ATTACKCW)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >= attackCost)
            {
                if (!amAI)
                {
                    playerCtrl.lastSelectedBuildingToBuild = carWashFighter;
                }
                else
                {

                }
            }
        }
        else if (tospawn == STORE.BOOSTCW)
        {
            if (GM.GetResouceCount((int)objID.ownerPlayerID) >= boostCost)
            {
                if (!amAI)
                {
                    playerCtrl.lastSelectedBuildingToBuild = carWashBoost;
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
                //victroy royale

            }
        }


        return (null);
    }
    private void Start()
    {
        objID = GetComponent<ObjectID>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GetComponent<Rigidbody>().Sleep();
        StartCoroutine(unitremover());
        playerCtrl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>();
    }

    private void FixedUpdate()
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


    }


    public IEnumerator unitremover()
    {

        while (true)
        {
            for (int i = 0; i < playerunit.Count; i++)
            {
                if (playerunit[i] == null)
                {
                    playerunit.RemoveAt(i);
                }
                yield return null;

            }
            unitCount = playerunit.Count;

            yield return null;
        }

    }
}
