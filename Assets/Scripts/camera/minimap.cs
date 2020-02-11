using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class minimap : MonoBehaviour
{
    void myMouseDown(PointerEventData dat)
    {
        Vector2 localCursor;
        var rect1 = GetComponent<RectTransform>();
        var pos1 = dat.position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, pos1, null, out localCursor))
        {
            int xpos = (int)localCursor.x;
            int ypos = (int)localCursor.y;

            if (xpos < 0)
            {
                xpos = xpos + (int)rect1.rect.width / 2;
            }
            else
            {
                xpos += (int)rect1.rect.width / 2;
            }

            if (ypos > 0)
            {
                ypos = ypos + (int)rect1.rect.height / 2;
            }
            else
            {
                ypos += (int)rect1.rect.height / 2;
            } 

            Debug.Log("Correct Cursor Pos: " + xpos + " " + ypos);
        }
    }
}
