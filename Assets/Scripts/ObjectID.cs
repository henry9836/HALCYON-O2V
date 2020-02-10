using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectID : MonoBehaviour
{
    public enum PlayerID
    {
        UNASSIGNED,
        PLAYER,
        AI
    }

    public enum OBJECTID
    {
        UNASSIGNED,
        UNIT,
        BUILDING
    }

    public PlayerID ownerPlayerID = PlayerID.UNASSIGNED;
    public OBJECTID objID = OBJECTID.UNASSIGNED;

    public float health = 100.0f;

}
