using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum ActionState
    {
        NONE,
        BUILDMODE,
        MOVEMODE
    }

    //Public
    public LayerMask unitInteractLayers;
    public float checkRadius = 10.0f;

    //Private
    private ActionState currentActionState = ActionState.NONE;
    private GameObject lastSelectedBuildingToBuild = null;
    private List<GameObject> selectedUnits = new List<GameObject>();
    

    public void assignNewUnits(List<GameObject> newUnits)
    {
        selectedUnits = newUnits;
    }

    public void updateBuildingToBuild(GameObject newBuild)
    {
        lastSelectedBuildingToBuild = newBuild;
    }

    private void FixedUpdate()
    {
        //Escape from mode and job
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentActionState = ActionState.NONE;
            selectedUnits.Clear();
            lastSelectedBuildingToBuild = null;
        }

        //Build mode
        if (lastSelectedBuildingToBuild != null)
        {
            currentActionState = ActionState.BUILDMODE;

            //Build a building

        }
        //If we have placed our building then revert to the none mode
        else if (currentActionState == ActionState.BUILDMODE)
        {
            currentActionState = ActionState.NONE;
        }

        //Move/Attack Mode
        if (selectedUnits.Count > 0)
        {
            currentActionState = ActionState.MOVEMODE;

            //Check for building tag near mouse right click
            if (Input.GetMouseButtonDown(1))
            {
                GameObject targetObj = null;

                //USe Vaughan;s Branch here
                Vector3 mouseInWorldPos = Vector3.zero;

                //Find nearby objs to mouse
                Collider[] cols = Physics.OverlapSphere(mouseInWorldPos, checkRadius, unitInteractLayers);
                if (cols.Length > 0)
                {
                    for (int i = 0; i < cols.Length; i++)
                    {
                        if (cols[i].gameObject.GetComponent<ObjectID>()!= null)
                        {
                            if (cols[i].gameObject.GetComponent<ObjectID>().ownerPlayerID != ObjectID.PlayerID.PLAYER)
                            {
                                targetObj = cols[i].gameObject;
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits[i].GetComponent<AIDroneController>().UpdateTargetPos(mouseInWorldPos, targetObj);
                }
            }
        }
        //We don't have any units selected
        else if (currentActionState == ActionState.BUILDMODE)
        {
            currentActionState = ActionState.NONE;
        }

    }
}
