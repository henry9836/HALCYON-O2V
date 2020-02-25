using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BHtimer : MonoBehaviour
{
    private Text thetext;
    private Blackhole BH;
    void Start()
    {
        thetext = this.gameObject.GetComponent<Text>();
        BH = GameObject.FindGameObjectWithTag("Blackhole").GetComponent<Blackhole>();
    }
    void Update()
    {
        thetext.text = "Certain Death: " +  (BH.twomintimer - BH.timer).ToString("F2");
    }
}
