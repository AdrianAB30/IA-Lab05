using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    [SerializeField] private NavMeshActionData actionData;
    [SerializeField] private float sampleRange = 2f;

    public NavMeshAgent agent;
    private NavMeshAction previousAction = NavMeshAction.None;

    private NavMeshPath debugPath;
    private Vector3 closestEdgePos;
    private Vector3 raycastHitPos;
    private Vector3 raycastEnd;
    private bool raycastHit;
    private Vector3 lastTarget;

    private void Awake()
    {
        actionData.action = NavMeshAction.None;
        previousAction = NavMeshAction.None;
    }

    void Update()
    {

        HandleInput();

        if (actionData.action != NavMeshAction.None && actionData.action != previousAction)
        {
            Debug.Log("Estado cambiado a: " + actionData.action);
            previousAction = actionData.action;

            switch (actionData.action)
            {
                case NavMeshAction.MoveToTargetPosition:
                    lastTarget = GetRandomDirectionPosition(3f);
                    MoveToTargetPosition(lastTarget);
                    break;

                case NavMeshAction.CalculatePath:
                    lastTarget = GetRandomDirectionPosition(3f);
                    CalculatePath(lastTarget);
                    MoveToTargetPosition(lastTarget);
                    break;

                case NavMeshAction.FindClosestEdge:
                    FindClosestEdge(transform.position);
                    agent.SetDestination(closestEdgePos);
                    break;

                case NavMeshAction.RayCast:
                    raycastEnd = transform.position + transform.forward * 5f;
                    RayCast(transform.position, raycastEnd);
                    break;
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) actionData.action = NavMeshAction.MoveToTargetPosition;
        if (Input.GetKeyDown(KeyCode.Alpha2)) actionData.action = NavMeshAction.CalculatePath;
        if (Input.GetKeyDown(KeyCode.Alpha3)) actionData.action = NavMeshAction.FindClosestEdge;
        if (Input.GetKeyDown(KeyCode.Alpha4)) actionData.action = NavMeshAction.RayCast;
    }

    public bool SamplePosition(Vector3 desiredPosition, float range, out Vector3 result)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(desiredPosition, out hit, range, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void MoveToTargetPosition(Vector3 desiredPosition)
    {
        Vector3 samplePos;
        if (SamplePosition(desiredPosition, sampleRange, out samplePos))
        {
            agent.SetDestination(samplePos);
        }
    }

    public void CalculatePath(Vector3 targetPosition)
    {
        debugPath = new NavMeshPath();
        if (agent.CalculatePath(targetPosition, debugPath))
        {
            agent.SetPath(debugPath);
        }
    }

    public void FindClosestEdge(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(position, out hit, NavMesh.AllAreas))
        {
            closestEdgePos = hit.position;
        }
    }

    public void RayCast(Vector3 start, Vector3 end)
    {
        NavMeshHit hit;
        if (NavMesh.Raycast(start, end, out hit, NavMesh.AllAreas))
        {
            raycastHit = true;
            raycastHitPos = hit.position;
            Debug.Log("Obstáculo encontrado entre los puntos.");
        }
        else
        {
            raycastHit = false;
            raycastHitPos = Vector3.zero;
            Debug.Log("El camino está libre entre los puntos.");
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        switch (actionData.action)
        {
            case NavMeshAction.MoveToTargetPosition:
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(lastTarget, 0.2f);
                break;

            case NavMeshAction.CalculatePath:
                if (debugPath != null && debugPath.corners.Length > 1)
                {
                    Gizmos.color = Color.yellow;
                    for (int i = 0; i < debugPath.corners.Length - 1; i++)
                    {
                        Gizmos.DrawLine(debugPath.corners[i], debugPath.corners[i + 1]);
                    }
                }
                break;

            case NavMeshAction.FindClosestEdge:
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, closestEdgePos);
                Gizmos.DrawSphere(closestEdgePos, 0.2f);
                break;

            case NavMeshAction.RayCast:
                Gizmos.color = raycastHit ? Color.red : Color.blue;
                Gizmos.DrawLine(transform.position, raycastHit ? raycastHitPos : raycastEnd);
                Gizmos.DrawSphere(raycastHit ? raycastHitPos : raycastEnd, 0.2f);
                break;
        }
    }
    private Vector3 GetRandomDirectionPosition(float distance)
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 direction = new Vector3(randomDir.x, 0f, randomDir.y);
        return transform.position + direction * distance;
    }
}