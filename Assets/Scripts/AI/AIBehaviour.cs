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
    public ObjectID objID;

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

            CheckForNulls();
        }
    }
}
