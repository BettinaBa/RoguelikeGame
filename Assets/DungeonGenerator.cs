using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public int steps = 50;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        Vector2Int currentPosition = Vector2Int.zero;
        floorPositions.Add(currentPosition);

        for (int i = 0; i < steps; i++)
        {
            currentPosition += GetRandomDirection();
            floorPositions.Add(currentPosition);
        }

        foreach (var pos in floorPositions)
        {
            Instantiate(floorPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);
        }

        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var pos in floorPositions)
        {
            foreach (Vector2Int dir in Directions())
            {
                Vector2Int wallPos = pos + dir;
                if (!floorPositions.Contains(wallPos))
                    wallPositions.Add(wallPos);
            }
        }

        foreach (var pos in wallPositions)
        {
            Instantiate(wallPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);
        }
    }

    Vector2Int GetRandomDirection()
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

    IEnumerable<Vector2Int> Directions()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
        yield return Vector2Int.right;
    }
}
