using UnityEngine;

public class PrefabPreviewer : MonoBehaviour
{
    public placeDelete placeDeleteScript;
    private GameObject previewInstance;
    private Material previewMaterial;
    public float previewTransparency = 0.3f;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isHoveringOverCube = Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Cube") && hit.collider.transform.childCount == 0;

        if (isHoveringOverCube)
        {
            GameObject hoveredCube = hit.collider.gameObject;
            if (previewInstance == null)
            {
                GameObject prefabToPreview = placeDeleteScript.objectToPlace;
                previewInstance = Instantiate(prefabToPreview, hoveredCube.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                previewMaterial = previewInstance.GetComponent<Renderer>().material;
                Color previewColor = previewMaterial.color;
                previewColor.a = previewTransparency;
                previewMaterial.color = previewColor;
            }
            else
            {
                Vector3 placementPosition = CalculatePlacementPosition(hoveredCube.transform.position);
                previewInstance.transform.position = placementPosition;
            }
        }
        else
        {
            if (previewInstance != null)
            {
                Destroy(previewInstance);
                previewInstance = null;
            }
        }
    }
    Vector3 CalculatePlacementPosition(Vector3 gridPosition)
    {
        Vector3 position = gridPosition;
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(gridPosition.x, gridPosition.y + 0.5f, gridPosition.z), Vector3.down);

        float highestPoint = gridPosition.y;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.point.y > highestPoint)
                {
                    highestPoint = hit.point.y;
                }
            }
        }
        position.y = highestPoint + placeDeleteScript.objectToPlace.GetComponent<Renderer>().bounds.extents.y;

        return position;
    }
}
