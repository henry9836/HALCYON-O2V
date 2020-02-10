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
    public LayerMask gridmask;
    public List<Vector3> points;
    public List<Vector3> orderedPoints;


    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            start = this.gameObject.GetComponent<mousepick>().getMousePos();
            startui = Input.mousePosition;
            pressed = true;

            this.gameObject.GetComponent<arrowmove>().movable = false;
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
            points.Add(end);

            Ray hitpoint = this.gameObject.GetComponent<Camera>().ScreenPointToRay(new Vector2(endui.x, startui.y));
            RaycastHit hit;
            if (Physics.Raycast(hitpoint.origin, hitpoint.direction, out hit, Mathf.Infinity, gridmask))
            {
                Debug.DrawLine(hit.point, hitpoint.origin);
                points.Add(hit.point);
            }

            hitpoint = this.gameObject.GetComponent<Camera>().ScreenPointToRay(new Vector2(startui.x, endui.y));

            if (Physics.Raycast(hitpoint.origin, hitpoint.direction, out hit, Mathf.Infinity, gridmask))
            {
                Debug.DrawLine(hit.point, hitpoint.origin);
                points.Add(hit.point);
            }


            selectunits(new Vector3(0.0f, 0.0f, 0.0f));

        }

        if (Input.GetMouseButtonUp(0) == true)
        {
            startui = Vector3.zero;
            endui = Vector3.zero;
            selectionUI.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, 0.0f);
            Debug.DrawLine(start, end, Color.red, 10.0f);
            pressed = false;
            this.gameObject.GetComponent<arrowmove>().movable = true;

        }
    }


    void selectunits(Vector3 testpos)
    {
        orderedPoints.Clear();

        //left
        float lowest = 999999;
        int lowestsave = -1;
        for (int i = 0; i < 4; i++)
        {
            if (lowest > points[i].x)
            {
                lowestsave = i;
                lowest = points[i].x;
            }
           
        }
        orderedPoints.Add(points[lowestsave]);

        //top
        lowest = -999999;
        lowestsave = -1;
        for (int i = 0; i < 4; i++)
        {
            if (lowest < points[i].z)
            {
                lowestsave = i;
                lowest = points[i].z;
            }

        }
        orderedPoints.Add(points[lowestsave]);

        //right
        lowest = -999999;
        lowestsave = -1;
        for (int i = 0; i < 4; i++)
        {
            if (lowest < points[i].x)
            {
                lowestsave = i;
                lowest = points[i].x;
            }

        }
        orderedPoints.Add(points[lowestsave]);

        //bottom
        lowest = 999999;
        lowestsave = -1;
        for (int i = 0; i < 4; i++)
        {
            if (lowest > points[i].z)
            {
                lowestsave = i;
                lowest = points[i].z;
            }

        }
        orderedPoints.Add(points[lowestsave]);


        Debug.DrawLine(orderedPoints[0], orderedPoints[1], Color.red, 1.0f);
        Debug.DrawLine(orderedPoints[1], orderedPoints[2], Color.red, 1.0f);
        Debug.DrawLine(orderedPoints[2], orderedPoints[3], Color.red, 1.0f);
        Debug.DrawLine(orderedPoints[3], orderedPoints[0], Color.red, 1.0f);
        Debug.DrawLine(orderedPoints[0], testpos, Color.red, 1.0f);
        Debug.DrawLine(orderedPoints[1], testpos, Color.red, 1.0f);
        Debug.DrawLine(orderedPoints[2], testpos, Color.red, 1.0f);
        Debug.DrawLine(orderedPoints[3], testpos, Color.red, 1.0f);



        float dist = Vector3.Distance(orderedPoints[0], orderedPoints[1]);
        float dist1 = Vector3.Distance(orderedPoints[1], orderedPoints[2]);
        float dist2 = Vector3.Distance(orderedPoints[2], orderedPoints[3]);
        float dist3 = Vector3.Distance(orderedPoints[3], orderedPoints[0]);

        float distt = Vector3.Distance(orderedPoints[0], testpos);
        float distt1 = Vector3.Distance(orderedPoints[1], testpos);
        float distt2 = Vector3.Distance(orderedPoints[2], testpos);
        float distt3 = Vector3.Distance(orderedPoints[3], testpos);

        float disttt = Vector3.Distance(orderedPoints[1], testpos);
        float disttt1 = Vector3.Distance(orderedPoints[2], testpos);
        float disttt2 = Vector3.Distance(orderedPoints[3], testpos);
        float disttt3 = Vector3.Distance(orderedPoints[0], testpos);

        float semiperim = (dist + distt + disttt) / 2;
        float semiperim1 = (dist1 + distt1 + disttt1) / 2;
        float semiperim2 = (dist2 + distt2 + disttt2) / 2;
        float semiperim3 = (dist3 + distt3 + disttt3) / 2;

        float aera = Mathf.Sqrt(semiperim * (semiperim - dist) * (semiperim - distt) * (semiperim - disttt));
        float aera1 = Mathf.Sqrt(semiperim1 * (semiperim1 - dist1) * (semiperim1 - distt1) * (semiperim1 - disttt1));
        float aera2 = Mathf.Sqrt(semiperim2 * (semiperim2 - dist2) * (semiperim2 - distt2) * (semiperim2 - disttt2));
        float aera3 = Mathf.Sqrt(semiperim3 * (semiperim3 - dist3) * (semiperim3 - distt3) * (semiperim3 - disttt3));

        float suspected = aera + aera1 + aera2 + aera3;
        
        

        Vector4 X = new Vector4(orderedPoints[0].x, orderedPoints[1].x, orderedPoints[2].x, orderedPoints[3].x);
        Vector4 Y = new Vector4(orderedPoints[0].z, orderedPoints[1].z, orderedPoints[2].z, orderedPoints[3].z);

        float area = 0.0f;
        int j = 3;
        for (int i = 0; i < 4; i++)
        {
            area += (X[j] + X[i]) * (Y[j] - Y[i]);
            j = i;  
        }

        float ans = Mathf.Abs(area / 2.0f);


        float scaleans = (100.0f / ans);
        float scalesus = scaleans * suspected;

        Debug.Log("triangels:" + scalesus + " square:" + ans + " diffrence:" + (scalesus - ans));

        if (suspected - ans <= 1.0f && ans != 0)
        {
            Debug.Log("cover");

        }

    }
}
