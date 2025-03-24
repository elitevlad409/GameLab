using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Color highlightEmission = Color.white * 2f;
    public Color occupiedHighlightEmission = Color.red * 2f;
    private GameObject lastHoveredCube;
    private Material[] lastMaterials; // Store materials for all highlighted cubes
    private Color[] originalEmissions; // Store original emission colors
    private bool[] hasEmissions; // Track emission property per cube
    public LayerMask cubeLayer;
    public placeDelete placeDeleteScript; // Reference to get objectToPlace
    public GridManager gridManager; // Reference to get grid positions

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
        {
            GameObject hoveredCube = hit.collider.gameObject;
            if (!hoveredCube.CompareTag("Cube") && !hoveredCube.CompareTag("OccupiedCube")) return;

            Vector2Int rootPos = gridManager.GetGridPosition(hoveredCube);
            if (rootPos.x == -1) return; // Invalid cube

            // Clear previous highlights
            ClearLastHighlight();

            // Determine size and cubes to highlight
            Vector2Int size;
            bool isOccupied = hoveredCube.transform.childCount > 0;
            if (isOccupied)
            {
                // Highlight the size of the placed object
                GameObject placedObj = hoveredCube.transform.GetChild(0).gameObject;
                PrefabSize prefabSize = placedObj.GetComponent<PrefabSize>();
                size = prefabSize != null ? prefabSize.size : new Vector2Int(1, 1);
            }
            else
            {
                // Highlight the size of the object to place
                PrefabSize prefabSize = placeDeleteScript.objectToPlace.GetComponent<PrefabSize>();
                size = prefabSize != null ? prefabSize.size : new Vector2Int(1, 1);
            }

            // Collect cubes to highlight
            GameObject[] cubesToHighlight = new GameObject[size.x * size.y];
            lastMaterials = new Material[size.x * size.y];
            originalEmissions = new Color[size.x * size.y];
            hasEmissions = new bool[size.x * size.y];
            int index = 0;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int gridPos = rootPos + new Vector2Int(x, y);
                    GameObject cube = gridManager.GetCubeAtGridPosition(gridPos);
                    if (cube != null && index < cubesToHighlight.Length)
                    {
                        cubesToHighlight[index] = cube;
                        Renderer cubeRenderer = cube.GetComponent<Renderer>();
                        Material cubeMaterial = cubeRenderer.material;
                        lastMaterials[index] = cubeMaterial;

                        if (cubeMaterial.HasProperty("_EmissionColor"))
                        {
                            originalEmissions[index] = cubeMaterial.GetColor("_EmissionColor");
                            hasEmissions[index] = true;
                            cubeMaterial.EnableKeyword("_EMISSION");
                            cubeMaterial.SetColor("_EmissionColor", isOccupied ? occupiedHighlightEmission : highlightEmission);
                        }
                        else
                        {
                            hasEmissions[index] = false;
                        }
                        index++;
                    }
                }
            }

            lastHoveredCube = hoveredCube; // Store root cube for clearing
        }
        else
        {
            ClearLastHighlight();
        }
    }

    void ClearLastHighlight()
    {
        if (lastHoveredCube != null && lastMaterials != null)
        {
            for (int i = 0; i < lastMaterials.Length; i++)
            {
                if (lastMaterials[i] != null && hasEmissions[i])
                {
                    lastMaterials[i].SetColor("_EmissionColor", originalEmissions[i]);
                    lastMaterials[i].DisableKeyword("_EMISSION");
                }
            }
        }
        lastHoveredCube = null;
        lastMaterials = null;
        originalEmissions = null;
        hasEmissions = null;
    }
}