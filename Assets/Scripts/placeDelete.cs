using UnityEngine;

public class placeDelete : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject objectToPlace;
    public GridManager gridManager; // Reference to GridManager
    private float placementHeightOffset = 0.5f;
    public LayerMask cubeLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryPlaceObject();
        }
        if (Input.GetMouseButtonDown(0))
        {
            TryRemoveObject();
        }
    }

    void TryPlaceObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
        {
            if (hit.collider.gameObject.CompareTag("Cube"))
            {
                GameObject rootCube = hit.collider.gameObject;
                Vector2Int rootPos = gridManager.GetGridPosition(rootCube);
                if (rootPos.x == -1) return; // Invalid cube

                PrefabSize prefabSize = objectToPlace.GetComponent<PrefabSize>();
                Vector2Int size = prefabSize != null ? prefabSize.size : new Vector2Int(1, 1);

                if (CanPlace(rootPos, size))
                {
                    Vector3 placementPosition = CalculatePlacementPosition(rootPos, size);
                    GameObject newObj = Instantiate(objectToPlace, placementPosition, Quaternion.identity);

                    // Parent to root cube and mark occupied cubes
                    for (int x = 0; x < size.x; x++)
                    {
                        for (int y = 0; y < size.y; y++)
                        {
                            Vector2Int gridPos = rootPos + new Vector2Int(x, y);
                            GameObject cube = gridManager.GetCubeAtGridPosition(gridPos);
                            if (cube != null)
                            {
                                newObj.transform.SetParent(cube.transform, true); // Parent to root cube
                                cube.tag = "OccupiedCube"; // Mark as occupied
                            }
                        }
                    }
                }
            }
        }
    }

    public bool CanPlace(Vector2Int rootPos, Vector2Int size)
    {
        // Check grid bounds
        if (rootPos.x + size.x > 10 || rootPos.y + size.y > 10)
            return false;

        // Check if all required squares are unoccupied
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkPos = rootPos + new Vector2Int(x, y);
                GameObject cube = gridManager.GetCubeAtGridPosition(checkPos);
                if (cube == null || cube.transform.childCount > 0)
                    return false;
            }
        }
        return true;
    }

    void TryRemoveObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
        {
            if (hit.collider.gameObject.CompareTag("Cube") || hit.collider.gameObject.CompareTag("OccupiedCube"))
            {
                GameObject cube = hit.collider.gameObject;
                if (cube.transform.childCount > 0)
                {
                    GameObject objToRemove = cube.transform.GetChild(0).gameObject;
                    PrefabSize prefabSize = objToRemove.GetComponent<PrefabSize>();
                    Vector2Int size = prefabSize != null ? prefabSize.size : new Vector2Int(1, 1);
                    Vector2Int rootPos = gridManager.GetGridPosition(cube);

                    // Clear all occupied cubes
                    for (int x = 0; x < size.x; x++)
                    {
                        for (int y = 0; y < size.y; y++)
                        {
                            Vector2Int gridPos = rootPos + new Vector2Int(x, y);
                            GameObject occupiedCube = gridManager.GetCubeAtGridPosition(gridPos);
                            if (occupiedCube != null && occupiedCube.transform.childCount > 0)
                            {
                                Destroy(occupiedCube.transform.GetChild(0).gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    public Vector3 CalculatePlacementPosition(Vector2Int rootPos, Vector2Int size)
    {
        Vector3 rootWorldPos = gridManager.GetWorldPosition(rootPos);
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(rootWorldPos.x, rootWorldPos.y + placementHeightOffset, rootWorldPos.z), Vector3.down, Mathf.Infinity, cubeLayer);

        float highestPoint = rootWorldPos.y;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Cube"))
            {
                if (hit.point.y > highestPoint)
                    highestPoint = hit.point.y;
            }
        }

        Vector3 position = rootWorldPos;
        position.y = highestPoint + objectToPlace.GetComponent<Renderer>().bounds.extents.y;
        position.x += (size.x - 1) * gridManager.gridSpacing * 0.5f; // Adjust for size
        position.z += (size.y - 1) * gridManager.gridSpacing * 0.5f; // Adjust for size
        return position;
    }
}