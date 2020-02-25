using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditPopUp : MonoBehaviour
{
    public GameObject Credits;
    public bool IsActive = false;

    public void popup()
    {
        if (IsActive == false) 
        {
            Credits.SetActive(true);
            IsActive = true;
        }
        else if (IsActive == true)
        {
            Credits.SetActive(false);
            IsActive = false;
        }
    }
}
