using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIetxt : MonoBehaviour
{
    public GameManager manager; 
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").gameObject.GetComponent<GameManager>();
    }

    void Update()
    {
        this.gameObject.GetComponent<Text>().text = "henry bux: " + manager.GetResouceCount((int)ObjectID.PlayerID.PLAYER).ToString();
    }
}
