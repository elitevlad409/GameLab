using UnityEngine;

public class placeDelete : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject objectToPlace;
    private float placementHeightOffset = 0.5f;

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

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Cube"))
            {
                GameObject cube = hit.collider.gameObject;
                if (cube.transform.childCount == 0)
                {
                    Vector3 placementPosition = CalculatePlacementPosition(cube.transform.position);
                    GameObject newObj = Instantiate(objectToPlace, placementPosition, Quaternion.identity);
                    newObj.transform.SetParent(cube.transform);
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
                if (cube.transform.childCount > 0)
                {
                    Destroy(cube.transform.GetChild(0).gameObject);
                }
            }
        }
    }

    Vector3 CalculatePlacementPosition(Vector3 gridPosition)
    {
        Vector3 position = gridPosition;
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(gridPosition.x, gridPosition.y + placementHeightOffset, gridPosition.z), Vector3.down);

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

        position.y = highestPoint + objectToPlace.GetComponent<Renderer>().bounds.extents.y;
        
        return position;
    }
}
