using UnityEngine;

public class placeDelete : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject objectToPlace;
    public GridManager gridManager;
    public GridRotator gridRotator;
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
                    Quaternion gridRotation = gridRotator.transform.rotation;
                    GameObject newObj = Instantiate(objectToPlace, placementPosition, gridRotation);

                    // Parent to root cube only and mark occupied cubes
                    newObj.transform.SetParent(rootCube.transform, true);
                    for (int x = 0; x < size.x; x++)
                    {
                        for (int y = 0; y < size.y; y++)
                        {
                            Vector2Int gridPos = rootPos + new Vector2Int(x, y);
                            GameObject cube = gridManager.GetCubeAtGridPosition(gridPos);
                            if (cube != null)
                            {
                                cube.tag = "OccupiedCube";
                            }
                        }
                    }
                }
            }
        }
    }

    public bool CanPlace(Vector2Int rootPos, Vector2Int size)
    {
        if (rootPos.x < 0 || rootPos.y < 0 || rootPos.x + size.x > 10 || rootPos.y + size.y > 10)
            return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int checkPos = rootPos + new Vector2Int(x, y);
                GameObject cube = gridManager.GetCubeAtGridPosition(checkPos);
                if (cube == null || cube.transform.childCount > 0)
                {
                    // Debug.Log($"Cannot place at {checkPos}: Cube={cube?.name}, ChildCount={cube?.transform.childCount}, Tag={cube?.tag}");
                    return false;
                }
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

                    // Destroy the object once from the root cube
                    Destroy(objToRemove);

                    // Reset all occupied cubes
                    for (int x = 0; x < size.x; x++)
                    {
                        for (int y = 0; y < size.y; y++)
                        {
                            Vector2Int gridPos = rootPos + new Vector2Int(x, y);
                            GameObject occupiedCube = gridManager.GetCubeAtGridPosition(gridPos);
                            if (occupiedCube != null)
                            {
                                occupiedCube.tag = "Cube"; // Reset tag
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
        return position;
    }
}