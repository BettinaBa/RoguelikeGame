using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public int width = 10;
    public int height = 10;

    void Start()
    {
        RegenerateRoom();
    }

    public void RegenerateRoom()
    {
        ClearChildren();
        GenerateRoom();
        GetComponent<NavMeshBaker>()?.Bake();
    }

    void GenerateRoom()
    {
        var floors = GenerateRoomData(width, height);
        foreach (var pos in floors)
        {
            Instantiate(
                floorPrefab,
                new Vector3(pos.x, pos.y, 1f),
                Quaternion.identity,
                transform
            );
        }
        GetComponent<NavMeshBaker>()?.Bake();
    }

    public static HashSet<Vector2Int> GenerateRoomData(int width, int height)
    {
        var floors = new HashSet<Vector2Int>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                floors.Add(new Vector2Int(x, y));
        return floors;
    }

    void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}
