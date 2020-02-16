﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject TCTemplate;
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

    private void Start()
    {
        GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPosition");

        for (int i = 0; i < MagicTraveller.TCCount; i++)
        {
            //Valid spawn position
            if (i <= spawnLocations.Length)
            {
                GameObject tmp = Instantiate(TCTemplate, spawnLocations[i].transform.position, Quaternion.identity);
                //Spawn a player
                if ((i + 1) <= MagicTraveller.PlayerCounter)
                {
                    tmp.GetComponent<ObjectID>().ownerPlayerID = ObjectID.PlayerID.PLAYER;
                }
                //Spawn an AI
                else
                {
                    tmp.AddComponent<AIBehaviour>();
                }
            }
            else
            {
                Debug.LogWarning("Tried to spawn a TC but there were no free spawn locations ignoring...");
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


}
