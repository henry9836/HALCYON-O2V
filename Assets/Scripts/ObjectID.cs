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

    public PlayerID ownerPlayerID = PlayerID.UNASSIGNED;
}
