using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showsMoney : MonoBehaviour
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
        float money = gamemanager.GetResouceCount(player.GetComponent<PlayerController>().playerID);
        this.gameObject.GetComponent<Text>().text = "$" + money + "m";
    }
}
