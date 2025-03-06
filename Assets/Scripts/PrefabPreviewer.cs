using UnityEngine;

public class PrefabPreviewer : MonoBehaviour
{
    public placeDelete placeDeleteScript; // Reference to placeDelete script
    private GameObject previewInstance; // Instance of the preview prefab
    private Material previewMaterial; // Material of the preview prefab
    public float previewTransparency = 0.3f; // How translucent the preview should be

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the raycast hits a valid grid cube
        bool isHoveringOverCube = Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Cube") && hit.collider.transform.childCount == 0;

        if (isHoveringOverCube)
        {
            GameObject hoveredCube = hit.collider.gameObject;

            // If we haven't instantiated the preview yet, instantiate it
            if (previewInstance == null)
            {
                // Get the currently selected prefab from the placeDelete script
                GameObject prefabToPreview = placeDeleteScript.objectToPlace;
                
                // Instantiate preview at the correct position
                previewInstance = Instantiate(prefabToPreview, hoveredCube.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                previewMaterial = previewInstance.GetComponent<Renderer>().material;

                // Set transparency of the preview material
                Color previewColor = previewMaterial.color;
                previewColor.a = previewTransparency; // Adjust transparency
                previewMaterial.color = previewColor;
            }
            else
            {
                // Update the position of the preview object
                Vector3 placementPosition = CalculatePlacementPosition(hoveredCube.transform.position);
                previewInstance.transform.position = placementPosition;
            }
        }
        else
        {
            // If not hovering over a grid cube, remove the preview
            if (previewInstance != null)
            {
                Destroy(previewInstance);
                previewInstance = null;
            }
        }
    }

    // Calculate the correct placement position for the preview object to avoid clipping
    Vector3 CalculatePlacementPosition(Vector3 gridPosition)
    {
        Vector3 position = gridPosition;

        // Raycast down from the center of the grid position
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(gridPosition.x, gridPosition.y + 0.5f, gridPosition.z), Vector3.down);

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
        position.y = highestPoint + placeDeleteScript.objectToPlace.GetComponent<Renderer>().bounds.extents.y;

        return position;
    }
}
