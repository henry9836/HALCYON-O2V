using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showUnits : MonoBehaviour
{
    private GameManager gamemanager;
    private GameObject player;
    public void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("MainCamera");
    }
    void FixedUpdate()
    {
        int curr = gamemanager.GetUnitCount(player.GetComponent<PlayerController>().playerID);
        int max = gamemanager.GetUnitCountMax(player.GetComponent<PlayerController>().playerID);

        this.gameObject.GetComponent<Text>().text = curr + "/" + max + " Units";
    }
}
