using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public Material buildAvailable;
    public Material buildNotAvailable;
    public GameObject VisualBuildFloor;

    //Private
    public ActionState currentActionState = ActionState.NONE;
    public GameObject lastSelectedBuildingToBuild = null;
    private GameObject spawnedObj = null;
    public List<GameObject> selectedUnits = new List<GameObject>();
    private Vector3 mouseInWorldPos = Vector3.zero;
    private mousepick mousePick;
    private MeshRenderer objMeshRenderer;
    private GameManager GM;

    public void assignNewUnits(List<GameObject> newUnits)
    {
        selectedUnits = newUnits;
    }

    public void updateBuildingToBuild(GameObject newBuild)
    {
        lastSelectedBuildingToBuild = newBuild;
    }

    bool isBuildSpot(Vector3 buildPos)
    {
        Collider[] cols = Physics.OverlapSphere(buildPos, 0.3f, unitInteractLayers);
        for (int i = 0; i < cols.Length; i++)
        {
            //Ignore Ourselves
            if (cols[i].gameObject != spawnedObj)
            {
                return false;
            }
        }


        return true;
    }

    Vector3 ConvertToSnapPosition(Vector3 rawVec)
    {
        Vector3 result = Vector3.zero;

        //Convert Down
        // (153 -> 1.53)
        //rawVec *= 0.01f;


        //Round off numbers
        //(1.53 -> 1.0)
        result = new Vector3(Mathf.RoundToInt(rawVec.x), Mathf.RoundToInt(rawVec.y), Mathf.RoundToInt(rawVec.z));

        //Convert Up
        //(1 -> 100)
        //result *= 100;

        return result;
    }

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerID = GM.RequestID((int)ObjectID.PlayerID.PLAYER);
        mousePick = GetComponent<mousepick>();
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
            VisualBuildFloor.SetActive(true);
            currentActionState = ActionState.BUILDMODE;

            //Build a building
            mouseInWorldPos = ConvertToSnapPosition(mousePick.getMousePos());

            //Have we spawned the preview Obj
            if (spawnedObj == null)
            {
                spawnedObj = Instantiate(lastSelectedBuildingToBuild, mouseInWorldPos, Quaternion.identity);
                objMeshRenderer = spawnedObj.GetComponent<MeshRenderer>();
                objMeshRenderer.material = buildNotAvailable;
                spawnedObj.layer = LayerMask.NameToLayer("NoCollide");
                spawnedObj.GetComponent<NavMeshObstacle>().enabled = false;
            }
            //Show in world and place if valid
            else
            {
                spawnedObj.transform.position = mouseInWorldPos;
                if (isBuildSpot(mouseInWorldPos))
                {
                    objMeshRenderer.material = buildAvailable;

                    //Build
                    if (Input.GetMouseButtonDown(0))
                    {
                        GM.UpdateResourceCount(playerID, -(spawnedObj.GetComponent<BuildingController>().costToBuild));
                        spawnedObj.GetComponent<BuildingController>().Placed();
                        lastSelectedBuildingToBuild = null;
                        spawnedObj = null;
                    }
                }
                else
                {
                    objMeshRenderer.material = buildNotAvailable;
                }
            }

        }
        //If we have placed our building then revert to the none mode
        else if (currentActionState == ActionState.BUILDMODE)
        {
            VisualBuildFloor.SetActive(false);
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
                mouseInWorldPos = mousePick.getMousePos();

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
                    if (selectedUnits[i].GetComponent<AIDroneController>() != null) {
                        selectedUnits[i].GetComponent<AIDroneController>().UpdateTargetPos(mouseInWorldPos, targetObj);
                    }
                }
            }
        }
        //We don't have any units selected
        else if (currentActionState != ActionState.BUILDMODE)
        {
            currentActionState = ActionState.NONE;
        }

    }
}
