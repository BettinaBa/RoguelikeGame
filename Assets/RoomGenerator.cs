using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public int width = 10;
    public int height = 10;

    public static (HashSet<Vector2Int> floors, HashSet<Vector2Int> walls) GenerateRoomData(int width, int height)
    {
        var floors = new HashSet<Vector2Int>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                floors.Add(new Vector2Int(x, y));

        var walls = new HashSet<Vector2Int>();
        for (int x = -1; x <= width; x++)
        {
            walls.Add(new Vector2Int(x, -1));
            walls.Add(new Vector2Int(x, height));
        }
        for (int y = 0; y < height; y++)
        {
            walls.Add(new Vector2Int(-1, y));
            walls.Add(new Vector2Int(width, y));
        }
        return (floors, walls);
    }

    void Start()
    {
        RegenerateRoom();
    }

    public void RegenerateRoom()
    {
        ClearChildren();
        GenerateRoom();
    }

    void GenerateRoom()
    {
        var data = GenerateRoomData(width, height);
        foreach (var pos in data.floors)
            Instantiate(floorPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);
        foreach (var pos in data.walls)
            Instantiate(wallPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);
    }

    void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}