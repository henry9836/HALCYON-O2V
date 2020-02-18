using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{

    public class scoutedObject
    {
        public scoutedObject(GameObject _obj)
        {
            obj = _obj;
            objID = obj.GetComponent<ObjectID>();
        }

        public GameObject obj;
        public ObjectID objID;
    }

    public int playerID = -1;

    private List<scoutedObject> foundObjs = new List<scoutedObject>();
    private float timer;
    private float spawnThreshold;
    public ObjectID objID;
    public GameObject TC;

    public void AddObjToList(GameObject obj)
    {
        //Check if we already know this obj
        for (int i = 0; i < foundObjs.Count; i++)
        {
            if (foundObjs[i].obj == obj)
            {
                return;
            }
        }

        //If we don't then add to list
        foundObjs.Add(new scoutedObject(obj));
    }

    void CheckForNulls()
    {
        for (int i = 0; i < foundObjs.Count; i++)
        {
            if (foundObjs[i].obj == null)
            {
                foundObjs.RemoveAt(i);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerID = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RequestID((int)ObjectID.PlayerID.AI_1);
        objID = GetComponent<ObjectID>();
        spawnThreshold = Random.Range(5.0f, 15.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (objID.ownerPlayerID == ObjectID.PlayerID.UNASSIGNED)
        {
            Debug.Log($"objID: {objID} = {objID.ownerPlayerID}");
            objID.ownerPlayerID = (ObjectID.PlayerID)playerID;
        }
        else
        {
            if (TC == null)
            {
                GameObject[] tcs = GameObject.FindGameObjectsWithTag("TC");

                for (int i = 0; i < tcs.Length; i++)
                {
                    if (tcs[i].GetComponent<ObjectID>().ownerPlayerID == objID.ownerPlayerID)
                    {
                        TC = tcs[i].gameObject;
                    }
                }

            }
            else
            {
                timer += Time.unscaledDeltaTime;

                if (timer >= spawnThreshold)
                {
                    spawnThreshold = Random.Range(5.0f, 15.0f);
                    TC.GetComponent<TCController>().SpawnUnit();
                    timer = 0.0f;
                }

                CheckForNulls();
            }
        }
    }
}
