using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject GameOver;
    public bool IsActive = false;

    public void GameOverpopup()
    {
        if (IsActive == false) 
        {
            GameOver.SetActive(true);
            IsActive = true;
        }
        else if (IsActive == true)
        {
            GameOver.SetActive(false);
            IsActive = false;
        }
    }
}
