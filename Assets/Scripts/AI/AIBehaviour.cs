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
        }

        public GameObject obj;
    };

    public List<scoutedObject> seenObjects = new List<scoutedObject>();
    


    public int playerID = -1;

    public void regNewSeenObject(GameObject obj)
    {
        //Filter out seen objects and add to list
        StartCoroutine(regNewSeenObjectCoroutine(obj));
    }

    void cullNulls()
    {
        //For all seen objs
        for (int i = 0; i < seenObjects.Count; i++)
        {
            //Is the obj null?
            if (seenObjects[i].obj == null)
            {
                //Remove obj if null
                seenObjects.RemoveAt(i);
            }
        }
    }

    void profitCheck()
    {

    }

    void attackLogic()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerID = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RequestID((int)ObjectID.PlayerID.AI_1);
        GetComponent<ObjectID>().ownerPlayerID = (ObjectID.PlayerID)playerID;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cullNulls();
        profitCheck();
        attackLogic();
    }

    IEnumerator regNewSeenObjectCoroutine(GameObject obj)
    {
        bool seen = false;
        //Have we seen this object?
        for (int i = 0; i < seenObjects.Count; i++)
        {
            if (seenObjects[i].obj == obj)
            {
                seen = true;
                break;
            }

            yield return null;
        }

        //Add to list
        if (!seen)
        {
            seenObjects.Add(new scoutedObject(obj));
        }

        yield return null;
    }
}
