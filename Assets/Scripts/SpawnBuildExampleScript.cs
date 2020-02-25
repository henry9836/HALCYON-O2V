using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuildExampleScript : MonoBehaviour
{

    public GameObject template = null;

    public void Spawn()
    {
        GetComponent<PlayerController>().lastSelectedBuildingToBuild = template;
    }

}
