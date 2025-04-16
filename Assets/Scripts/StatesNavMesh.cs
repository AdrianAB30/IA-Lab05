using UnityEngine;

[CreateAssetMenu(fileName = "StatesNavMesh", menuName = "States")]
public class NavMeshActionData : ScriptableObject
{
    public NavMeshAction action;    
}

public enum NavMeshAction
{
    None,
    MoveToTargetPosition,
    CalculatePath,
    FindClosestEdge,
    RayCast
}