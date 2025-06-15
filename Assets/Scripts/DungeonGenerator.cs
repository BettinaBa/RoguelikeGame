using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public int steps = 50;
    public bool randomSeed = true;
    public int seed = 0;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

    public static (HashSet<Vector2Int> floors, HashSet<Vector2Int> walls) GenerateDungeonData(int steps, bool randomSeed, int seed)
    {
        if (randomSeed)
            Random.InitState(System.DateTime.Now.GetHashCode());
        else
            Random.InitState(seed);

        HashSet<Vector2Int> floorPositions = new();
        Vector2Int currentPosition = Vector2Int.zero;
        floorPositions.Add(currentPosition);

        for (int i = 0; i < steps; i++)
        {
            currentPosition += GetRandomDirection();
            floorPositions.Add(currentPosition);
        }

        HashSet<Vector2Int> wallPositions = new();
        foreach (var pos in floorPositions)
        {
            foreach (Vector2Int dir in Directions())
            {
                Vector2Int wallPos = pos + dir;
                if (!floorPositions.Contains(wallPos))
                    wallPositions.Add(wallPos);
            }
        }

        return (floorPositions, wallPositions);
    }

    void Start()
    {
        ClearChildren();
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        var data = GenerateDungeonData(steps, randomSeed, seed);
        floorPositions = data.floors;

        foreach (var pos in data.floors)
            Instantiate(floorPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);

        foreach (var pos in data.walls)
            Instantiate(wallPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);

        GetComponent<NavMeshBaker>()?.Bake();
    }

    void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        floorPositions.Clear();
    }

    static Vector2Int GetRandomDirection()
    {
        int index = Random.Range(0, 4);
        switch (index)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            default: return Vector2Int.right;
        }
    }

    static IEnumerable<Vector2Int> Directions()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
        yield return Vector2Int.right;
    }
}
