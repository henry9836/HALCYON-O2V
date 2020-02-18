using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExampleScript : MonoBehaviour
{

    GameObject TC = null;

    public void Spawn()
    {
        if (TC == null)
        {
            int playerID = GetComponent<PlayerController>().playerID;

            GameObject[] tcs = GameObject.FindGameObjectsWithTag("TC");

            for (int i = 0; i < tcs.Length; i++)
            {
                if (tcs[i].GetComponent<ObjectID>().ownerPlayerID == (ObjectID.PlayerID)playerID)
                {
                    TC = tcs[i].gameObject;
                }
            }
        }

        TC.GetComponent<TCController>().SpawnUnit();

    }

}
