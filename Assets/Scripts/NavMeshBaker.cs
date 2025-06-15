using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour
{
    NavMeshSurface surface;

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    /// <summary>
    /// Builds the NavMesh for the attached NavMeshSurface.
    /// </summary>
    public void Bake()
    {
        if (surface != null)
            surface.BuildNavMesh();
    }
}
