using System.Collections.Generic;
using UnityEngine;

public class MapChunkManager : MonoBehaviour
{
    public static MapChunkManager Instance { get; private set; }

    public Transform player;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public int chunkSize = 10;
    public int loadRadius = 2;

    private readonly Dictionary<Vector2Int, GameObject> chunks = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        UpdateChunks();
    }

    void Update()
    {
        UpdateChunks();
    }

    void UpdateChunks()
    {
        if (player == null)
            return;
        Vector2 playerPos = player.position;
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / chunkSize),
            Mathf.FloorToInt(playerPos.y / chunkSize));
        HashSet<Vector2Int> needed = new();
        for (int dx = -loadRadius; dx <= loadRadius; dx++)
            for (int dy = -loadRadius; dy <= loadRadius; dy++)
                needed.Add(new Vector2Int(playerChunk.x + dx, playerChunk.y + dy));

        foreach (Vector2Int coord in needed)
        {
            if (!chunks.ContainsKey(coord))
                CreateChunk(coord);
        }

        List<Vector2Int> remove = new();
        foreach (var kv in chunks)
        {
            if (!needed.Contains(kv.Key))
                remove.Add(kv.Key);
        }
        foreach (Vector2Int coord in remove)
        {
            Destroy(chunks[coord]);
            chunks.Remove(coord);
        }
    }

    void CreateChunk(Vector2Int coord)
    {
        GameObject chunkParent = new GameObject($"Chunk_{coord.x}_{coord.y}");
        chunkParent.transform.parent = transform;
        chunkParent.transform.position = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0f);
        chunks[coord] = chunkParent;

        var data = RoomGenerator.GenerateRoomData(chunkSize, chunkSize);
        foreach (var pos in data.floors)
            Instantiate(floorPrefab, chunkParent.transform.position + new Vector3(pos.x, pos.y, 1f), Quaternion.identity, chunkParent.transform);
        foreach (var pos in data.walls)
            Instantiate(wallPrefab, chunkParent.transform.position + new Vector3(pos.x, pos.y, 1f), Quaternion.identity, chunkParent.transform);
    }

    public Bounds GetLoadedBounds()
    {
        if (chunks.Count == 0)
            return new Bounds();
        Vector2 min = new(float.MaxValue, float.MaxValue);
        Vector2 max = new(float.MinValue, float.MinValue);
        foreach (Vector2Int coord in chunks.Keys)
        {
            Vector2 origin = new Vector2(coord.x * chunkSize, coord.y * chunkSize);
            min = Vector2.Min(min, origin);
            max = Vector2.Max(max, origin + new Vector2(chunkSize, chunkSize));
        }
        return new Bounds((min + max) / 2f, max - min);
    }
}
