using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// Removes any NavMesh-related components from loaded scenes at runtime.
/// This ensures the game does not rely on Unity's NavMesh system.
/// </summary>
public static class NavMeshStripper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded += (scene, mode) => RemoveNavMeshComponents();
        RemoveNavMeshComponents();
    }

    static void RemoveNavMeshComponents()
    {
        foreach (var agent in Object.FindObjectsOfType<NavMeshAgent>())
        {
            Object.Destroy(agent);
        }
        foreach (var obstacle in Object.FindObjectsOfType<NavMeshObstacle>())
        {
            Object.Destroy(obstacle);
        }
    }
}
