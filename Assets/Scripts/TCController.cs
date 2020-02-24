using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCController : MonoBehaviour
{
    public GameObject unitTemplate;

    public float baseCost = 100;

    private bool registered = false;
    private ObjectID objID;
    private GameManager GM;
    public List<GameObject> playerunit = new List<GameObject>();


    
    public GameObject SpawnUnit()
    {
        GameManager manager = GameObject.Find("GameManger").GetComponent<GameManager>();
        int player = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().playerID;

        if ((manager.GetResouceCount(player) >= baseCost) && (manager.GetUnitCount(player) < manager.GetUnitCountMax(player)))
        {
            manager.UpdateResourceCount(player, -baseCost);
            GameObject spawnedObj = Instantiate(unitTemplate, transform.position, Quaternion.identity);
            spawnedObj.GetComponent<ObjectID>().ownerPlayerID = objID.ownerPlayerID;

            playerunit.Add(spawnedObj);
            return spawnedObj;
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
                    i--;
                }
            }
            yield return null;
        }
    }
}
