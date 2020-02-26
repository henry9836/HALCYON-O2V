using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher : MonoBehaviour
{
    public float Timer;
    public float rate = 5.0f;
    public GameObject astroid;
    public Vector3 forceDirStart;
    public Vector3 forceDirEnd;
    public float range;
    public float force;

    private Blackhole blackHole;

    private void Start()
    {
        blackHole = GameObject.FindGameObjectWithTag("Blackhole").GetComponent<Blackhole>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer > rate)
        {
            Vector3 range1 = new Vector3(transform.position.x + Random.Range(range, -range), transform.position.y, transform.position.z + Random.Range(range, -range));

            Timer = 0.0f;
            GameObject temp = Instantiate(astroid, new Vector3(range1.x, range1.y, range1.z), Quaternion.identity);
            float t = (blackHole.timer / (blackHole.twomintimer * 0.5f));
            //temp.GetComponent<Rigidbody>().AddForce(Vector3.Lerp(forceDirStart.normalized, forceDirEnd.normalized, (blackHole.timer / (blackHole.twomintimer * 0.5f))) * force, ForceMode.Impulse);
            temp.GetComponent<Rigidbody>().AddForce(new Vector3(Mathf.Lerp(forceDirStart.normalized.x, forceDirEnd.normalized.x, t) , Mathf.Lerp(forceDirStart.normalized.y, forceDirEnd.normalized.y, t), Mathf.Lerp(forceDirStart.normalized.z, forceDirEnd.normalized.z, t)) * force, ForceMode.Impulse);
        }
    }
}
