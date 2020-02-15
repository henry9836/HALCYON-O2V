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
    public float checkRadius = 2.0f;
    public int playerID = -1;

    //Private
    public ActionState currentActionState = ActionState.NONE;
    private GameObject lastSelectedBuildingToBuild = null;
    public List<GameObject> selectedUnits = new List<GameObject>();



    public void assignNewUnits(List<GameObject> newUnits)
    {
        selectedUnits = newUnits;
    }

    public void updateBuildingToBuild(GameObject newBuild)
    {
        lastSelectedBuildingToBuild = newBuild;
    }

    void Start()
    {
        playerID = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RequestID((int)ObjectID.PlayerID.PLAYER);
    }

    private void Update()
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

                //Get The mouse position in world
                Vector3 mouseInWorldPos = GetComponent<mousepick>().getMousePos();

                //Find nearby objs to mouse
                Collider[] cols = Physics.OverlapSphere(mouseInWorldPos, checkRadius, unitInteractLayers);
                if (cols.Length > 0)
                {
                    for (int i = 0; i < cols.Length; i++)
                    {
                        if (cols[i].gameObject.GetComponent<ObjectID>()!= null)
                        {
                            //Do not target our own buildings
                            if (cols[i].gameObject.GetComponent<ObjectID>().ownerPlayerID != ObjectID.PlayerID.PLAYER)
                            {
                                targetObj = cols[i].gameObject;
                                break;
                            }
                            //Useless they are damaged
                            else if (cols[i].gameObject.GetComponent<ObjectID>().health != cols[i].gameObject.GetComponent<ObjectID>().maxHealth)
                            {
                                targetObj = cols[i].gameObject;
                                break;
                            }
                        }
                        else
                        {
                            Debug.LogWarning(cols[i].name + " is missing an Object ID component");
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
