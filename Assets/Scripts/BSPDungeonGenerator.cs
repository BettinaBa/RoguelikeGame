using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    [Header("Map Size")]
    public int width = 50;
    public int height = 50;

    [Header("Room Settings")]
    public int minRoomSize = 6;
    public int maxRoomSize = 12;
    public int maxDepth = 4;

    [Header("Random Seed")]
    public bool randomSeed = true;
    public int seed = 0;

    class Node
    {
        public RectInt rect;
        public RectInt room;
        public Node left;
        public Node right;

        public Node(RectInt rect)
        {
            this.rect = rect;
        }

        public bool IsLeaf => left == null && right == null;

        public Vector2Int RoomCenter
        {
            get
            {
                if (room.width == 0 || room.height == 0)
                    return Vector2Int.zero;
                return new Vector2Int(
                    room.x + room.width / 2,
                    room.y + room.height / 2
                );
            }
        }
    }

    void Start()
    {
        ClearChildren();
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        var data = GenerateDungeonData(width, height, minRoomSize, maxRoomSize, maxDepth, randomSeed, seed);

        foreach (var pos in data.floors)
            Instantiate(floorPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);

        foreach (var pos in data.walls)
            Instantiate(wallPrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity, transform);
    }

    static IEnumerable<Vector2Int> Directions()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.down;
        yield return Vector2Int.left;
        yield return Vector2Int.right;
    }

    void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    public static (HashSet<Vector2Int> floors, HashSet<Vector2Int> walls) GenerateDungeonData(
        int width,
        int height,
        int minRoomSize,
        int maxRoomSize,
        int maxDepth,
        bool randomSeed,
        int seed)
    {
        if (randomSeed)
            Random.InitState(System.DateTime.Now.GetHashCode());
        else
            Random.InitState(seed);

        Node root = new Node(new RectInt(0, 0, width, height));
        Split(root, 0, maxDepth, minRoomSize);
        CreateRooms(root, minRoomSize, maxRoomSize);

        HashSet<Vector2Int> floors = new HashSet<Vector2Int>();
        HashSet<Vector2Int> walls = new HashSet<Vector2Int>();

        PopulateTiles(root, floors);
        GenerateCorridors(root, floors);

        foreach (var pos in floors)
        {
            foreach (var dir in Directions())
            {
                Vector2Int wp = pos + dir;
                if (!floors.Contains(wp))
                    walls.Add(wp);
            }
        }

        return (floors, walls);
    }

    static void Split(Node node, int depth, int maxDepth, int minSize)
    {
        if (depth >= maxDepth ||
            node.rect.width < minSize * 2 ||
            node.rect.height < minSize * 2)
            return;

        bool splitHoriz = node.rect.width > node.rect.height ? true :
            (node.rect.width < node.rect.height ? false : Random.value > 0.5f);

        if (splitHoriz)
        {
            int splitX = Random.Range(minSize, node.rect.width - minSize);
            node.left = new Node(new RectInt(node.rect.x, node.rect.y, splitX, node.rect.height));
            node.right = new Node(new RectInt(node.rect.x + splitX, node.rect.y, node.rect.width - splitX, node.rect.height));
        }
        else
        {
            int splitY = Random.Range(minSize, node.rect.height - minSize);
            node.left = new Node(new RectInt(node.rect.x, node.rect.y, node.rect.width, splitY));
            node.right = new Node(new RectInt(node.rect.x, node.rect.y + splitY, node.rect.width, node.rect.height - splitY));
        }

        Split(node.left, depth + 1, maxDepth, minSize);
        Split(node.right, depth + 1, maxDepth, minSize);
    }

    static void CreateRooms(Node node, int minSize, int maxSize)
    {
        if (!node.IsLeaf)
        {
            CreateRooms(node.left, minSize, maxSize);
            CreateRooms(node.right, minSize, maxSize);
            return;
        }

        int roomWidth = Random.Range(minSize, Mathf.Min(maxSize, node.rect.width - 2) + 1);
        int roomHeight = Random.Range(minSize, Mathf.Min(maxSize, node.rect.height - 2) + 1);
        int roomX = Random.Range(node.rect.x + 1, node.rect.x + node.rect.width - roomWidth - 1);
        int roomY = Random.Range(node.rect.y + 1, node.rect.y + node.rect.height - roomHeight - 1);
        node.room = new RectInt(roomX, roomY, roomWidth, roomHeight);
    }

    static void PopulateTiles(Node node, HashSet<Vector2Int> floors)
    {
        if (node == null)
            return;
        if (node.IsLeaf)
        {
            for (int x = node.room.x; x < node.room.xMax; x++)
                for (int y = node.room.y; y < node.room.yMax; y++)
                    floors.Add(new Vector2Int(x, y));
        }
        else
        {
            PopulateTiles(node.left, floors);
            PopulateTiles(node.right, floors);
        }
    }

    static void GenerateCorridors(Node node, HashSet<Vector2Int> floors)
    {
        if (node == null || node.IsLeaf)
            return;

        GenerateCorridors(node.left, floors);
        GenerateCorridors(node.right, floors);

        Vector2Int leftCenter = GetRoomCenter(node.left);
        Vector2Int rightCenter = GetRoomCenter(node.right);

        foreach (var pos in CreateCorridor(leftCenter, rightCenter))
            floors.Add(pos);
    }

    static Vector2Int GetRoomCenter(Node node)
    {
        if (node.IsLeaf)
            return node.RoomCenter;
        return GetRoomCenter(Random.value > 0.5f ? node.left : node.right);
    }

    static IEnumerable<Vector2Int> CreateCorridor(Vector2Int a, Vector2Int b)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        Vector2Int pos = a;
        while (pos.x != b.x)
        {
            corridor.Add(pos);
            pos += (b.x > pos.x) ? Vector2Int.right : Vector2Int.left;
        }
        while (pos.y != b.y)
        {
            corridor.Add(pos);
            pos += (b.y > pos.y) ? Vector2Int.up : Vector2Int.down;
        }
        corridor.Add(b);
        return corridor;
    }
}
