using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class minimap : MonoBehaviour, IPointerClickHandler
{
    public LayerMask grid;
    private Vector3 campos;

    void Start()
    {
        campos = GameObject.Find("minimapcam").gameObject.transform.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CallOnPointerClickEvent(eventData);
    }
    

    void CallOnPointerClickEvent(PointerEventData dat)
    {
        Vector2 localCursor;
        RectTransform rect1 = GetComponent<RectTransform>();
        Vector2 pos1 = dat.position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, pos1, null, out localCursor))
        {
            float xpos = localCursor.x;
            float ypos = localCursor.y;

            if (xpos < 0)
            {
                xpos = xpos + rect1.rect.width / 2;
            }
            else
            {
                xpos += rect1.rect.width / 2;
            }

            if (ypos > 0)
            {
                ypos = ypos + rect1.rect.height / 2;
            }
            else
            {
                ypos += rect1.rect.height / 2;
            } 

            Debug.Log("Correct Cursor Pos: " + xpos + " " + ypos);

            movemaincam(new Vector2(xpos, ypos) * 0.2f);
        }
    }

    public void movemaincam(Vector2 pos)
    {
        Vector3 position = (Quaternion.AngleAxis(45.0f, Vector3.up) * (new Vector3(campos.x + pos.x, campos.y, campos.z + pos.y) - campos)) + campos;

        RaycastHit hitUI;
        if (Physics.Raycast(position, -Vector3.up, out hitUI, grid))
        {
            
            Debug.DrawLine(position, hitUI.point, Color.red, 10.0f);
        }

        GameObject cam = GameObject.Find("Main Camera");
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, grid))
        {

            Debug.DrawLine(position, hit.point, Color.red, 10.0f);
        }

        cam.gameObject.transform.position += hitUI.point - hit.point; 
    }

}
