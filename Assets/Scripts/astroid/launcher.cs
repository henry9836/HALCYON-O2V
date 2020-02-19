using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher : MonoBehaviour
{
    public float Timer;
    public float rate = 5.0f;
    public GameObject astroid;
    public Vector3 force;

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer > rate)
        {
            Timer = 0.0f;
            GameObject temp = Instantiate(astroid, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }
}
