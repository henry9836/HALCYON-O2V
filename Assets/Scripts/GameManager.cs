using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class Bank
    {
        public int ID;
        public float resourceCount;

        public Bank(int _ID, float _startingResource)
        {
            ID = _ID;
            resourceCount = _startingResource;
        }

    }

    int nextID = 0;
    int tcCount = 0;
    public float defaultResources = 50.0f;
    public List<Bank> banks = new List<Bank>();
    public List<GameObject> playerTCs = new List<GameObject>();
    public List<GameObject> aiTCs = new List<GameObject>();

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

    private void FixedUpdate()
    {
        //Once tcs are registered
        if (tcCount >= 2)
        {
            //Check who is alive
            //Player lost
            if (playerTCs.Count <= 0)
            {
                Debug.Log("AI WINS");
            }
            //AI lost
            else if (aiTCs.Count <= 0)
            {
                Debug.Log("PLAYER WINS");
            }
        }

        CheckListsForNulls();
    }


}
