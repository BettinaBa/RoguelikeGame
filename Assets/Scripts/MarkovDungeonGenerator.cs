using System.Collections.Generic;
using UnityEngine;

public class MarkovDungeonGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    [Header("Grid Size")]
    public int width = 20;
    public int height = 20;

    [System.Serializable]
    public class AdjacencyProbability
    {
        [Tooltip("Adjacency pattern as UDLR with 1 for floor, 0 for wall")]
        public string pattern = "0000";
        [Range(0f, 1f)] public float floorChance = 0.5f;
    }

    [Header("Probability Table")]
    public List<AdjacencyProbability> probabilityTable = new List<AdjacencyProbability>();

    int[,] grid;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        ClearChildren();
        GenerateGrid();
        InstantiateTiles();
    }

    void GenerateGrid()
    {
        grid = new int[width, height];
        var lookup = new Dictionary<string, float>();
        foreach (var entry in probabilityTable)
        {
            if (entry.pattern.Length == 4)
                lookup[entry.pattern] = Mathf.Clamp01(entry.floorChance);
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                string pat = GetPattern(x, y);
                float chance = lookup.ContainsKey(pat) ? lookup[pat] : 0.5f;
                grid[x, y] = Random.value < chance ? 1 : 0;
            }
        }
    }

    string GetPattern(int x, int y)
    {
        char up = y < height - 1 && grid[x, y + 1] == 1 ? '1' : '0';
        char down = y > 0 && grid[x, y - 1] == 1 ? '1' : '0';
        char left = x > 0 && grid[x - 1, y] == 1 ? '1' : '0';
        char right = x < width - 1 && grid[x + 1, y] == 1 ? '1' : '0';
        return $"{up}{down}{left}{right}";
    }

    void InstantiateTiles()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject prefab = grid[x, y] == 1 ? floorPrefab : wallPrefab;
                if (prefab != null)
                    Instantiate(prefab, new Vector3(x, y, 1f), Quaternion.identity, transform);
            }
        }
    }

    void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}
