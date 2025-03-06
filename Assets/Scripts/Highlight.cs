using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Color highlightEmission = Color.white * 2f; // Color when cube is free
    public Color occupiedHighlightEmission = Color.red * 2f; // Color when cube is occupied by prefab
    private GameObject lastHoveredCube;
    private Material lastMaterial;
    private Color originalEmission;
    private bool hasEmission;
    public LayerMask cubeLayer; // Assign the "Cubes" layer in Inspector

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Use the layer mask to ONLY hit cubes, ignoring placed objects
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
        {
            GameObject hoveredCube = hit.collider.gameObject;

            Renderer cubeRenderer = hoveredCube.GetComponent<Renderer>();
            Material cubeMaterial = cubeRenderer.material;

            // Reset previous cube emission
            if (lastHoveredCube != null && lastHoveredCube != hoveredCube)
            {
                lastMaterial.SetColor("_EmissionColor", originalEmission);
                lastMaterial.DisableKeyword("_EMISSION");
            }

            // Store original emission color
            if (lastHoveredCube != hoveredCube)
            {
                if (cubeMaterial.HasProperty("_EmissionColor"))
                {
                    originalEmission = cubeMaterial.GetColor("_EmissionColor");
                    hasEmission = true;
                }
                else
                {
                    hasEmission = false;
                }
            }

            // Apply highlight
            if (hoveredCube.transform.childCount > 0) // Cube has prefab on it
            {
                // Use different color for occupied cubes
                if (hasEmission)
                {
                    cubeMaterial.EnableKeyword("_EMISSION");
                    cubeMaterial.SetColor("_EmissionColor", occupiedHighlightEmission);
                }
            }
            else
            {
                // Use default highlight color for empty cubes
                if (hasEmission)
                {
                    cubeMaterial.EnableKeyword("_EMISSION");
                    cubeMaterial.SetColor("_EmissionColor", highlightEmission);
                }
            }

            lastMaterial = cubeMaterial;
            lastHoveredCube = hoveredCube;
            return;
        }

        // Reset emission if no cube is hovered
        if (lastHoveredCube != null && hasEmission)
        {
            lastMaterial.SetColor("_EmissionColor", originalEmission);
            lastMaterial.DisableKeyword("_EMISSION");
            lastHoveredCube = null;
        }
    }
}
