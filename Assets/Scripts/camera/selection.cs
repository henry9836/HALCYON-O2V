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

    public List<Vector3> points;

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

            selectionUI.gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(startui.x / Screen.width, startui.y / Screen.height);
            selectionUI.gameObject.GetComponent<RectTransform>().anchorMax = selectionUI.gameObject.GetComponent<RectTransform>().anchorMin;


            if (startui.x > endui.x)
            {
                if (startui.y > endui.y)
                {
                    selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(endui.x - startui.x), Mathf.Abs(endui.y - startui.y));
                    selectionUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.x / 2), -(selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2));

                }
                else
                {
                    selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(endui.x - startui.x), Mathf.Abs(endui.y - startui.y));
                    selectionUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.x / 2), (selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2));
                }
            }
            else
            {
                if (startui.y > endui.y)
                {
                    selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(endui.x - startui.x), Mathf.Abs(endui.y - startui.y));
                    selectionUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.x / 2), -(selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2));

                }
                else
                {
                    selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(endui.x - startui.x), Mathf.Abs(endui.y - startui.y));
                    selectionUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.x / 2), (selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2));
                }
            }

            end = this.gameObject.GetComponent<mousepick>().getMousePos();

            points.Clear();
            points.Add(start);
            //points.Add(); other 2 corners after raycasting
            points.Add(end);

        }

        if (Input.GetMouseButtonUp(0) == true)
        {
            startui = Vector3.zero;
            endui = Vector3.zero;
            selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, 0.0f);
            Debug.DrawLine(start, end, Color.red, 10.0f);
            pressed = false;
        }
    }


    void selectunits()
    { 
    
    
    }
}
