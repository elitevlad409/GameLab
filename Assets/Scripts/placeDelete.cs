using UnityEngine;

public class placeDelete : MonoBehaviour
{
    public GameObject objectToPlace; // Assign this in the Inspector

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
                    GameObject newObj = Instantiate(objectToPlace, cube.transform.position + Vector3.up * 0.5f, Quaternion.identity);
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
}
