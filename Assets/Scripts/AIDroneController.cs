using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIDroneController : MonoBehaviour
{

    public enum Type
    {
        UNASSIGNED,
        WORKER
    };

    public enum AttackStance
    {
        ATTACK,
        HOLDGROUND,
        NOATTACK
    }


    public Type aiType = Type.UNASSIGNED;
    public AttackStance attackState = AttackStance.ATTACK;
    public float agroRange = 10.0f;
    public float attackRange = 2.0f;
    public float attackCoolDown = 0.5f;
    public float attackDamage = 7.0f;

    private NavMeshAgent agent;
    private GameObject targetGameObject = null;
    private LayerMask unitInteractLayers;
    private bool aiIsStuck = false;
    private float attackTimer = 0.0f;
    private bool targetIsBuilding = false;

    public void TakeDamage(float dmg)
    {
        GetComponent<ObjectID>().health -= dmg;
    }

    public void UpdateTargetPos(Vector3 targetPos, GameObject obj)
    {
        NavMeshPath path = new NavMeshPath();

        //Go To Gameobject
        if (obj != null)
        {
            targetPos = obj.transform.position;
            targetGameObject = obj;
        }
        else
        {
            targetGameObject = null;
        }
        //Go To Point
        agent.CalculatePath(targetPos, path);
        agent.SetPath(path);
    }

    void DealDamage()
    {
        Debug.Log("Dealing Damage To: " + gameObject.name);
        if (targetGameObject.GetComponent<ObjectID>().objID == ObjectID.OBJECTID.BUILDING)
        {
            targetIsBuilding = true;
        }
        else
        {
            targetIsBuilding = false;
        }
        //Is Unit
        if (!targetIsBuilding)
        {
            targetGameObject.GetComponent<AIDroneController>().TakeDamage(attackDamage);
        }
        //Is Building
        else
        {

        }
    }

    void Attack(GameObject obj)
    {
        //If we are not at our attack threshold distance
        if (Vector3.Distance(transform.position, obj.transform.position) > attackRange)
        {
            UpdateTargetPos(Vector3.zero, obj);
            Debug.Log(gameObject.name + " MOVE");
        }
        //Wait at our position and attack don't need to get closer
        else{
            agent.isStopped = true;
            agent.ResetPath();
            Debug.Log(gameObject.name + " STOP");

            //Deal Damage
            if (attackTimer > attackCoolDown)
            {
                attackTimer = 0.0f;
                DealDamage();
            }

        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        unitInteractLayers = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerController>().unitInteractLayers;
    }

    private void FixedUpdate()
    {
        attackTimer += Time.unscaledDeltaTime;

        //Are we dead?
        if (GetComponent<ObjectID>().health <= 0)
        {
            Destroy(gameObject);
        }

        //Check if we stopped but have a path still pending
        if (agent.pathStatus == NavMeshPathStatus.PathPartial && (agent.velocity.magnitude < 0.1f))
        {
            Debug.Log("AI is stuck :(");
            aiIsStuck = true;
        }
        //We recovered from stuck position
        else if (agent.pathStatus == NavMeshPathStatus.PathPartial && (agent.velocity.magnitude > 0.1f))
        {
            aiIsStuck = false;
        }
        //Fallback
        else if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            aiIsStuck = false;
        }

        //Idle/Stuck Attack Logic
        //If we are stuck or we have stopped and we are not currently attacking anything
        if ((aiIsStuck || agent.velocity.magnitude < 0.1f) && attackState == AttackStance.ATTACK && targetGameObject == null)
        {
            //Look for a target
            Collider[] cols = Physics.OverlapSphere(transform.position, agroRange, unitInteractLayers);
            if (cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].gameObject.GetComponent<ObjectID>() != null)
                    {
                        //Do not target ourselves
                        if (cols[i].gameObject.GetComponent<ObjectID>().ownerPlayerID != GetComponent<ObjectID>().ownerPlayerID)
                        {
                            Attack(cols[i].gameObject);
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning(cols[i].name + " is missing an Object ID component");
                    }
                }
            }
        }

        //If we have an attack target and it's not dead
        if (targetGameObject != null)
        {
            Debug.Log(gameObject.name + " attack that obj");
            Attack(targetGameObject);
        }

        if (GetComponent<ObjectID>().ownerPlayerID == ObjectID.PlayerID.PLAYER)
        {
            if (targetGameObject != null)
            {
                Debug.LogWarning(Vector3.Distance(targetGameObject.transform.position, transform.position));
            }
        }

    }

    

}
