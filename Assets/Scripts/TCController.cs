using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCController : MonoBehaviour
{
    public GameObject unitTemplate;

    public float baseCost = 100;
    public float mineCost = 5000;
    public float attackCost = 100000;
    public float boostCost = 150000;
    public float escapeCost = 999999;

    private bool registered = false;
    private ObjectID objID;
    private GameManager GM;
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

    public GameObject SpawnUnit(TCController.STORE tospawn)
    {
        if (tospawn == STORE.BASE)
        {
            if ((GM.GetResouceCount((int)objID.ownerPlayerID) >= baseCost) && (GM.GetUnitCount((int)objID.ownerPlayerID) < GM.GetUnitCountMax((int)objID.ownerPlayerID)))
            {
                Debug.Log("spawn unit");
                GM.UpdateResourceCount((int)objID.ownerPlayerID, -baseCost);
                GameObject spawnedObj = Instantiate(unitTemplate, transform.position, Quaternion.identity);
                spawnedObj.GetComponent<ObjectID>().ownerPlayerID = objID.ownerPlayerID;
                playerunit.Add(spawnedObj);
                GM.setUnitCount((int)objID.ownerPlayerID, playerunit.Count);
                return spawnedObj;
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
