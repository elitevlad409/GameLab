using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject[,] gridCubes = new GameObject[10, 10]; // 10x10 grid
    public float gridSpacing = 0.98f; // Adjusted based on your cube positions
    private Vector3 gridOrigin;

    void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        Debug.Log("Found " + cubes.Length + " cubes tagged 'Cube'");

        if (cubes.Length != 100)
        {
            Debug.LogError("Expected 100 grid cubes, found " + cubes.Length);
            return;
        }

        // Find grid origin (bottom-left corner)
        float minX = float.MaxValue, minZ = float.MaxValue;
        foreach (GameObject cube in cubes)
        {
            Vector3 pos = cube.transform.position;
            if (pos.x < minX) minX = pos.x;
            if (pos.z < minZ) minZ = pos.z;
        }
        gridOrigin = new Vector3(minX, cubes[0].transform.position.y, minZ);
        Debug.Log("Grid Origin: " + gridOrigin);

        // Assign cubes to grid
        foreach (GameObject cube in cubes)
        {
            Vector3 relativePos = cube.transform.position - gridOrigin;
            // Use float division and clamp to avoid overflow
            float floatX = relativePos.x / gridSpacing;
            float floatZ = relativePos.z / gridSpacing;
            int x = Mathf.Clamp(Mathf.RoundToInt(floatX), 0, 9);
            int z = Mathf.Clamp(Mathf.RoundToInt(floatZ), 0, 9);

            Debug.Log($"Cube {cube.name}: Pos={cube.transform.position}, RelativePos={relativePos}, GridPos=({x},{z})");

            if (gridCubes[x, z] == null)
            {
                gridCubes[x, z] = cube;
            }
            else
            {
                Debug.LogWarning($"Duplicate cube at grid position ({x},{z}): {cube.name}");
            }
        }

        // Verify grid population
        int populatedCount = 0;
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                if (gridCubes[x, z] != null) populatedCount++;
            }
        }
        Debug.Log("Populated " + populatedCount + " out of 100 grid slots");
    }

    public Vector2Int GetGridPosition(GameObject cube)
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                if (gridCubes[x, z] == cube)
                    return new Vector2Int(x, z);
            }
        }
        return new Vector2Int(-1, -1);
    }

    public GameObject GetCubeAtGridPosition(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < 10 && pos.y >= 0 && pos.y < 10)
            return gridCubes[pos.x, pos.y];
        return null;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        GameObject cube = GetCubeAtGridPosition(gridPos);
        return cube != null ? cube.transform.position : Vector3.zero;
    }
}