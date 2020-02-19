using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HPcolor : MonoBehaviour
{
    public Color MaxHP;
    public Color MinHP;

    void Update()
    {
        float fillammount = this.gameObject.GetComponent<Image>().fillAmount;
        float r = Mathf.Lerp(MinHP.r, MaxHP.r, fillammount);
        float g = Mathf.Lerp(MinHP.g, MaxHP.g, fillammount);
        float b = Mathf.Lerp(MinHP.b, MaxHP.b, fillammount);
        float a = Mathf.Lerp(MinHP.a, MaxHP.a, fillammount);

        this.gameObject.GetComponent<Image>().color = new Color(r, g, b, a);
    }
}    
