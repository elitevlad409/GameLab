using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectRotator : MonoBehaviour
{
    public placeDelete placeDeleteScript; // To toggle functionality
    public GridRotator gridRotator; // To get grid rotation
    public OutlineManager outlineManager; // To interact with outlines
    public Canvas uiCanvas; // Reference to the Canvas
    public float rotationSpeed = 50f; // Smooth rotation speed
    public Button toggleModeButton; // UI button to switch modes
    public Button rotateLeftButton; // UI button to rotate left
    public Button rotateRightButton; // UI button to rotate right
    public Button resetRotationButton; // UI button to reset to grid rotation
    public Button snap90Button; // UI button to snap 90°
    public LayerMask cubeLayer;

    private bool isRotateMode = false; // Current mode (false = place/delete, true = rotate)
    private GameObject selectedObject; // Currently selected object for rotation
    private Vector3 rotationDirection = Vector3.zero; // Direction for smooth rotation
    private Color originalOutlineColor; // Store original outline color
    private RectTransform uiButtonsRect; // Parent for rotation buttons

    void Start()
    {
        // Setup toggle button
        toggleModeButton.onClick.AddListener(ToggleMode);

        // Create UI buttons parent (dynamically positioned)
        GameObject uiButtons = new GameObject("RotationButtons");
        uiButtonsRect = uiButtons.AddComponent<RectTransform>();
        uiButtonsRect.SetParent(uiCanvas.transform, false); // Use Canvas reference
        uiButtonsRect.sizeDelta = new Vector2(200, 100); // Adjust size as needed
        uiButtonsRect.gameObject.SetActive(false); // Hidden by default

        // Position buttons within the parent
        SetupButton(rotateLeftButton, new Vector2(-80, 30), "Rotate Left", RotateLeft);
        SetupButton(rotateRightButton, new Vector2(80, 30), "Rotate Right", RotateRight);
        SetupButton(resetRotationButton, new Vector2(-80, -30), "Reset", ResetRotation);
        SetupButton(snap90Button, new Vector2(80, -30), "Snap 90°", Snap90);

        // Ensure placeDelete starts active
        placeDeleteScript.enabled = true;
        this.enabled = false; // Start in place/delete mode
    }

    void Update()
    {
        if (!isRotateMode) return;

        // Smooth rotation
        if (selectedObject != null && rotationDirection != Vector3.zero)
        {
            selectedObject.transform.Rotate(rotationDirection * rotationSpeed * Time.deltaTime, Space.World);
        }

        // Handle left-click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if clicking a UI button (ignore object selection/deselection)
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
            {
                GameObject clickedCube = hit.collider.gameObject;
                if (clickedCube.CompareTag("OccupiedCube") && clickedCube.transform.childCount > 0)
                {
                    GameObject newSelected = clickedCube.transform.GetChild(0).gameObject;
                    if (newSelected != selectedObject)
                    {
                        DeselectObject();
                        SelectObject(newSelected);
                    }
                }
                else
                {
                    DeselectObject();
                }
            }
            else
            {
                DeselectObject();
            }
        }
    }

    void ToggleMode()
    {
        isRotateMode = !isRotateMode;
        placeDeleteScript.enabled = !isRotateMode;
        this.enabled = isRotateMode;
        toggleModeButton.GetComponentInChildren<TMP_Text>().text = isRotateMode ? "Place/Delete Mode" : "Rotate Mode";
        DeselectObject();
    }

    void SelectObject(GameObject obj)
    {
        selectedObject = obj;
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            originalOutlineColor = outline.OutlineColor; // Store current color
            outline.OutlineColor = Color.white; // Set to white
            // Note: Thickness is managed by OutlineManager
        }

        // Position UI buttons above the object
        Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
        uiButtonsRect.position = screenPos;
        uiButtonsRect.gameObject.SetActive(true);
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            Outline outline = selectedObject.GetComponent<Outline>();
            if (outline != null)
            {
                outline.OutlineColor = originalOutlineColor; // Restore original color
            }
            selectedObject = null;
            rotationDirection = Vector3.zero;
            uiButtonsRect.gameObject.SetActive(false);
        }
    }

    void RotateLeft() { rotationDirection = Vector3.down; }
    void RotateRight() { rotationDirection = Vector3.up; }
    void ResetRotation()
    {
        if (selectedObject != null)
        {
            selectedObject.transform.rotation = gridRotator.transform.rotation;
            rotationDirection = Vector3.zero;
        }
    }

    void Snap90()
    {
        if (selectedObject != null)
        {
            selectedObject.transform.Rotate(Vector3.up * 90f, Space.World);
            rotationDirection = Vector3.zero;
        }
    }

    void SetupButton(Button button, Vector2 position, string label, UnityEngine.Events.UnityAction action)
    {
        button.transform.SetParent(uiButtonsRect, false);
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(70, 30);
        button.GetComponentInChildren<Text>().text = label;
        button.onClick.AddListener(action);
    }
}