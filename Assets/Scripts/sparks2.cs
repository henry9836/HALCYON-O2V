using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sparks2 : MonoBehaviour
{

    public void call()
    {
        StartCoroutine(particle());

    }

    public IEnumerator particle()
    {
        Debug.Log("2");

        yield return new WaitForSeconds(2.0f);
        Debug.Log("3");

        Destroy(this.gameObject);
        Debug.Log("4");

    }
}
