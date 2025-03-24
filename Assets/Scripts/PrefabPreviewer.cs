using UnityEngine;

public class PrefabPreviewer : MonoBehaviour
{
    public placeDelete placeDeleteScript;
    private GameObject previewInstance;
    private Material previewMaterial;
    public float previewTransparency = 0.3f;
    public LayerMask cubeLayer;

    void Start()
    {
        CreatePreviewInstance();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isHoveringOverCube = Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer) &&
                                 hit.collider.gameObject.CompareTag("Cube");

        if (isHoveringOverCube)
        {
            GameObject hoveredCube = hit.collider.gameObject;
            Vector2Int rootPos = placeDeleteScript.gridManager.GetGridPosition(hoveredCube);
            if (rootPos.x == -1) return; // Invalid cube

            PrefabSize prefabSize = placeDeleteScript.objectToPlace.GetComponent<PrefabSize>();
            Vector2Int size = prefabSize != null ? prefabSize.size : new Vector2Int(1, 1);

            if (placeDeleteScript.CanPlace(rootPos, size))
            {
                Vector3 placementPosition = placeDeleteScript.CalculatePlacementPosition(rootPos, size);
                previewInstance.transform.position = placementPosition;
                previewInstance.transform.rotation = placeDeleteScript.gridRotator.transform.rotation;
                previewInstance.SetActive(true);
            }
            else
            {
                previewInstance.SetActive(false);
            }
        }
        else
        {
            previewInstance.SetActive(false);
        }
    }

    void CreatePreviewInstance()
    {
        if (previewInstance != null) Destroy(previewInstance);
        GameObject prefabToPreview = placeDeleteScript.objectToPlace;
        previewInstance = Instantiate(prefabToPreview, Vector3.zero, placeDeleteScript.gridRotator.transform.rotation);
        previewMaterial = previewInstance.GetComponent<Renderer>().material;
        Color previewColor = previewMaterial.color;
        previewColor.a = previewTransparency;
        previewMaterial.color = previewColor;
        previewInstance.SetActive(false);
    }

    public void UpdatePreviewPrefab()
    {
        CreatePreviewInstance();
    }
}