using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public int playerID = -1;

    // Start is called before the first frame update
    void Start()
    {
        playerID = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RequestID((int)ObjectID.PlayerID.AI_1);
        GetComponent<ObjectID>().ownerPlayerID = (ObjectID.PlayerID)playerID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
