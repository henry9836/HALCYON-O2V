using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExampleScript : MonoBehaviour
{
    GameObject TC = null;

    public void Spawnbase()
    {
        fixTC();
        TC.GetComponent<TCController>().SpawnUnit(TCController.STORE.BASE);
    }

    public void Spawnminecw()
    {
        fixTC();
        TC.GetComponent<TCController>().SpawnUnit(TCController.STORE.MINECW);
    }
    public void Spawnattack()
    {
        fixTC();
        TC.GetComponent<TCController>().SpawnUnit(TCController.STORE.ATTACKCW);
    }

    public void Spawnboost()
    {
        fixTC();
        TC.GetComponent<TCController>().SpawnUnit(TCController.STORE.BOOSTCW);
    }

    public void Spawnescape()
    {
        fixTC();
        TC.GetComponent<TCController>().SpawnUnit(TCController.STORE.ESCAPE);
    }

    public void fixTC()
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

    }

}
