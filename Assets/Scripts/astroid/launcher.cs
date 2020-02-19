using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher : MonoBehaviour
{
    public float Timer;
    public float rate = 5.0f;
    public GameObject astroid;
    public Vector3 force;
    public float range;

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer > rate)
        {
            Vector3 range1 = new Vector3(transform.position.x + Random.Range(range, -range), transform.position.y, transform.position.z + Random.Range(range, -range));

            Timer = 0.0f;
            GameObject temp = Instantiate(astroid, new Vector3(range1.x, range1.y, range1.z), Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
    }
}
