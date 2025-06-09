using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
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
    }

    void GenerateRoom()
    {
        // Floor
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Instantiate(floorPrefab, new Vector3(x, y, 1f), Quaternion.identity, transform);

        // Walls
        for (int x = -1; x <= width; x++)
        {
            Instantiate(wallPrefab, new Vector3(x, -1, 1f), Quaternion.identity, transform);
            Instantiate(wallPrefab, new Vector3(x, height, 1f), Quaternion.identity, transform);
        }
        for (int y = 0; y < height; y++)
        {
            Instantiate(wallPrefab, new Vector3(-1, y, 1f), Quaternion.identity, transform);
            Instantiate(wallPrefab, new Vector3(width, y, 1f), Quaternion.identity, transform);
        }
    }

    void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}