using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selection : MonoBehaviour
{
    bool pressed = false;
    public Vector3 start;
    public Vector3 end;
    public Vector2 startui;
    public Vector2 endui;
    public GameObject selectionUI;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            start = this.gameObject.GetComponent<mousepick>().getMousePos();
            startui = Input.mousePosition;
            pressed = true;
        }

        if (pressed == true)
        {
            endui = Input.mousePosition;

            if (startui.x > endui.x)
            {
                selectionUI.gameObject.GetComponent<RectTransform>().localPosition = startui;
                selectionUI.gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);

            }
            else
            {

            }

            if (startui.y > endui.y)
            {

            }
            else
            {
            }
        }



        if (Input.GetMouseButtonUp(0) == true)
        {
            end = this.gameObject.GetComponent<mousepick>().getMousePos();
            startui = Vector3.zero;
            endui = Vector3.zero;
            Debug.DrawLine(start, end, Color.red, 10.0f);
            pressed = false;
        }
    }
}
