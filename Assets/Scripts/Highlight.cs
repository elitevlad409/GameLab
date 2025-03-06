using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Color highlightEmission = Color.white * 2f;
    public Color occupiedHighlightEmission = Color.red * 2f;
    private GameObject lastHoveredCube;
    private Material lastMaterial;
    private Color originalEmission;
    private bool hasEmission;
    public LayerMask cubeLayer;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
        {
            GameObject hoveredCube = hit.collider.gameObject;

            Renderer cubeRenderer = hoveredCube.GetComponent<Renderer>();
            Material cubeMaterial = cubeRenderer.material;
            if (lastHoveredCube != null && lastHoveredCube != hoveredCube)
            {
                lastMaterial.SetColor("_EmissionColor", originalEmission);
                lastMaterial.DisableKeyword("_EMISSION");
            }

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
            if (hoveredCube.transform.childCount > 0)
            {
                if (hasEmission)
                {
                    cubeMaterial.EnableKeyword("_EMISSION");
                    cubeMaterial.SetColor("_EmissionColor", occupiedHighlightEmission);
                }
            }
            else
            {
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
        if (lastHoveredCube != null && hasEmission)
        {
            lastMaterial.SetColor("_EmissionColor", originalEmission);
            lastMaterial.DisableKeyword("_EMISSION");
            lastHoveredCube = null;
        }
    }
}
