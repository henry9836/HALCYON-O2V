using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeFlash : MonoBehaviour
{
    public bool flashing = false;
    public Color fincol;
    public Color startcol;

    private float timer;
    public float speed;
    private float resettime;



    void Update()
    {
        if (flashing == true)
        {
            timer += Time.deltaTime;
            if (timer > resettime)
            {
                resettime = (1.0f / speed) * 2.0f;
                timer = 0.0f;
                StartCoroutine(flash());
            }
        }
    }

    public IEnumerator flash()
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed)
        {
            float r = Mathf.Lerp(startcol.r, fincol.r, t);
            float g = Mathf.Lerp(startcol.g, fincol.g, t);
            float b = Mathf.Lerp(startcol.b, fincol.b, t);
            float a = Mathf.Lerp(startcol.a, fincol.a, t);

            this.gameObject.GetComponent<Image>().color = new Color(r, g, b, a);

            yield return null;
        }
        for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime * speed)
        {
            float r = Mathf.Lerp(startcol.r, fincol.r, t);
            float g = Mathf.Lerp(startcol.g, fincol.g, t);
            float b = Mathf.Lerp(startcol.b, fincol.b, t);
            float a = Mathf.Lerp(startcol.a, fincol.a, t);

            this.gameObject.GetComponent<Image>().color = new Color(r, g, b, a);

            yield return null;
        }


        yield return null;

    }
}
