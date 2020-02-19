using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCController : MonoBehaviour
{
    private bool registered = false;
    private ObjectID objID;
    private GameManager GM;
    private void Start()
    {
        objID = GetComponent<ObjectID>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
