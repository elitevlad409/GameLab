using UnityEngine;

public class placeDelete : MonoBehaviour
{
    public GameObject[] prefabs; // Array of prefabs to place
    public GameObject objectToPlace; // Current object to place
    private float placementHeightOffset = 0.5f; // Offset for placing objects above grid cubes

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click to place
        {
            TryPlaceObject();
        }
        if (Input.GetMouseButtonDown(0)) // Left-click to delete
        {
            TryRemoveObject();
        }
    }

    void TryPlaceObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Cube"))
            {
                GameObject cube = hit.collider.gameObject;

                // Check if cube already has an object
                if (cube.transform.childCount == 0)
                {
                    // Calculate the position where the prefab should be placed
                    Vector3 placementPosition = CalculatePlacementPosition(cube.transform.position);
                    
                    // Instantiate the prefab at the calculated position
                    GameObject newObj = Instantiate(objectToPlace, placementPosition, Quaternion.identity);
                    newObj.transform.SetParent(cube.transform); // Attach to cube
                }
            }
        }
    }

    void TryRemoveObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Cube"))
            {
                GameObject cube = hit.collider.gameObject;

                // Remove object if it exists
                if (cube.transform.childCount > 0)
                {
                    Destroy(cube.transform.GetChild(0).gameObject);
                }
            }
        }
    }

    // Calculate the correct placement position for the prefab to avoid clipping
    Vector3 CalculatePlacementPosition(Vector3 gridPosition)
    {
        Vector3 position = gridPosition;
        
        // Raycast down from the center of the grid position
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(gridPosition.x, gridPosition.y + placementHeightOffset, gridPosition.z), Vector3.down);

        float highestPoint = gridPosition.y;

        // Loop through all raycast hits and find the highest point
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                // Update the highest point if a collision is detected
                if (hit.point.y > highestPoint)
                {
                    highestPoint = hit.point.y;
                }
            }
        }

        // Set the placement position to just above the highest point
        position.y = highestPoint + objectToPlace.GetComponent<Renderer>().bounds.extents.y;
        
        return position;
    }
}
