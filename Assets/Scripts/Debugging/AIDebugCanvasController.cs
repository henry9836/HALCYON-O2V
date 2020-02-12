using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIDebugCanvasController : MonoBehaviour
{

    public List<GameObject> TextObjs = new List<GameObject>();
    public GameObject debugTarget;

    private AIDroneController aiDroneCtrl;
    private ObjectID objID;
    private bool visualActive = false;

    //Owner
    //Health
    //Attack mode
    //[State]
    //Targeting and info
    //Idle
    //Stuck

    private void Start()
    {
        aiDroneCtrl = transform.parent.GetComponent<AIDroneController>();
        objID = transform.parent.GetComponent<ObjectID>();
        visualActive = false;
    }

    void FixedUpdate()
    {
        if (aiDroneCtrl.DebugMode)
        {
            if (TextObjs.Count < 4)
            {
                Debug.LogWarning("Debug Canvas missing text objects");
            }
            else
            {
                if (!visualActive)
                {
                    for (int i = 0; i < TextObjs.Count; i++)
                    {
                        TextObjs[i].SetActive(true);
                    }
                    visualActive = true;
                }

                //Update text
                //Owner
                TextObjs[0].GetComponent<Text>().text = "OWNER {" + objID.ownerPlayerID.ToString() + "}";
                if (objID.ownerPlayerID == ObjectID.PlayerID.UNASSIGNED)
                {
                    TextObjs[0].GetComponent<Text>().color = Color.magenta;
                }
                else if (objID.ownerPlayerID == ObjectID.PlayerID.UNASSIGNED)
                {
                    TextObjs[0].GetComponent<Text>().color = Color.red;
                }
                else if (objID.ownerPlayerID == ObjectID.PlayerID.UNASSIGNED)
                {
                    TextObjs[0].GetComponent<Text>().color = Color.blue;
                }
                //Health
                TextObjs[1].GetComponent<Text>().text = "HEALTH: " + objID.health.ToString() + "/" + objID.maxHealth.ToString();

                AIDroneController.aiDebug debugClass = aiDroneCtrl.returnDebug();
                //State
                TextObjs[2].GetComponent<Text>().text = "ATTACK STATE {" + debugClass.attackState.ToString() + "}";

                //If we are attacking
                if (debugClass.target.hasTarget() && !debugClass.stuck)
                {
                    if (debugClass.target.hasTargetObj())
                    {
                        TextObjs[3].GetComponent<Text>().text = "ACTION { ATTACKING OBJECT: " + debugClass.target.tarObject.name + "\n AT POSITION: " + debugClass.target.tarObjectAdjustPos + "}";
                        debugTarget.transform.position = debugClass.target.tarObjectAdjustPos;
                        debugTarget.SetActive(true);
                    }
                    else if(debugClass.target.hasTargetPos())
                    {
                        TextObjs[3].GetComponent<Text>().text = "ACTION { MOVING TO: " + debugClass.target.tarPos + "}";
                        debugTarget.transform.position = debugClass.target.tarPos;
                        debugTarget.SetActive(true);
                    }
                    else
                    {
                        TextObjs[3].GetComponent<Text>().text = "";

                        debugTarget.SetActive(false);
                    }
                }
                //If we are idle and stuck
                else if (debugClass.idle && debugClass.stuck)
                {
                    TextObjs[3].GetComponent<Text>().text = "STATE: STUCK/IDLE";
                    debugTarget.SetActive(false);
                }
                //If we are idle
                else {
                    TextObjs[3].GetComponent<Text>().text = "STATE: IDLE";
                    debugTarget.SetActive(false);
                }
            }
        }
        else
        {
            if (visualActive)
            {
                for (int i = 0; i < TextObjs.Count; i++)
                {
                    TextObjs[i].SetActive(false);
                }
                debugTarget.SetActive(false);
                visualActive = false;
            }
        }
    }
}
