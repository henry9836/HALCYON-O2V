using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectID : MonoBehaviour
{
    public enum PlayerID
    {
        UNASSIGNED,
        PLAYER,
        AI_1,
        AI_2,
        AI_3,
        AI_4,
        AI_5,
        AI_6,
        AI_7,
        AI_8,
    }

    public enum OBJECTID
    {
        UNASSIGNED,
        UNIT,
        BUILDING,
        RESOURCE
    }

    public PlayerID ownerPlayerID = PlayerID.UNASSIGNED;
    public OBJECTID objID = OBJECTID.UNASSIGNED;
    public float health = 100.0f;
    public float maxHealth = 100.0f;
    public float velo = 0.0f;

    private float startMaxHealth = 100.0f;

    public void ModifyMaxHealth(float newMax)
    {
        float percentageOfCurrentHealth = health / maxHealth;
        maxHealth = newMax;
        health = newMax * percentageOfCurrentHealth;
    }

    public void ResetHealthToOriginal()
    {
        ModifyMaxHealth(startMaxHealth);
    }

    private void Start()
    {
        startMaxHealth = maxHealth;
    }

    void Update()
    {
        if (objID == OBJECTID.UNIT) {
            this.gameObject.transform.GetChild(0).GetComponentInChildren<Image>().fillAmount = health / maxHealth;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (maxHealth < health)
        {
            health = maxHealth;
        }
    }

}
