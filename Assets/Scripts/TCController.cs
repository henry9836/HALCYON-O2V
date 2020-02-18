using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCController : MonoBehaviour
{

    public float costOfUnit = 10.0f;
    public GameObject unit;

    private bool registered = false;
    private ObjectID objID;
    private GameManager GM;

    private void Start()
    {
        objID = GetComponent<ObjectID>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void SpawnUnit()
    {
        if (GM.GetResouceCount((int)objID.ownerPlayerID) >= costOfUnit)
        {
            GameObject tmp = Instantiate(unit, transform.position, Quaternion.identity);
            tmp.GetComponent<ObjectID>().ownerPlayerID = objID.ownerPlayerID;
        }
    }

    private void FixedUpdate()
    {
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
}
