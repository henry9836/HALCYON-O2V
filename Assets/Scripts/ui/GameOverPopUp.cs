using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject GameOver;
    public bool IsActive = false;

    public void GameOverpopup()
    {
        GameOver.SetActive(!IsActive);
        IsActive = !IsActive;
    }
}
