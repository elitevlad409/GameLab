using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OutlineManager : MonoBehaviour
{
    public FlexibleColorPicker fcp;
    public Slider thicknessSlider;
    public Image colorPreview;
    public GameObject settingsPanel;
    public Color currentColor;

    private List<Outline> allOutlines = new List<Outline>(); // Stores all Outline components

    void Start()
    {
        // Initialize the slider and add a listener to update thickness
        thicknessSlider.onValueChanged.AddListener(UpdateOutlineThickness);
        // Initialize the color picker
        if (fcp != null)
        {
            fcp.onColorChange.AddListener(UpdateOutlineColor);
        }
    }

    void Update()
    {
        // Continuously find new objects with Outline components
        RefreshOutlines();
    }

    void RefreshOutlines()
    {
        // Find all objects in the scene with Outline components
        Outline[] foundOutlines = FindObjectsOfType<Outline>();
        Debug.Log("Found " + foundOutlines.Length + " outlines");

        // Only add new outlines that are not already tracked
        foreach (Outline outline in foundOutlines)
        {
            if (!allOutlines.Contains(outline))
            {
                allOutlines.Add(outline);
                ApplyCurrentOutlineSettings(outline);
            }
        }
    }

    void ApplyCurrentOutlineSettings(Outline outline)
    {
        // Apply the current thickness and color to a single outline
        outline.OutlineWidth = thicknessSlider.value;
        outline.OutlineColor = fcp.color;
    }

    void UpdateOutlineThickness(float value)
    {
        // Update thickness for all outlines
        foreach (Outline outline in allOutlines)
        {
            outline.OutlineWidth = value;
        }
    }

    public void UpdateOutlineColor(Color newColor)
    {
        // Update color for all outlines
        foreach (Outline outline in allOutlines)
        {
            outline.OutlineColor = newColor;
            currentColor = newColor;
        }

        // Update color preview UI
        if (colorPreview != null)
        {
            colorPreview.color = newColor;
        }
    }

    public void OpenPanel()
    {
        settingsPanel.SetActive(true);

        // Update color preview in case it's different from currentColor
        if (colorPreview != null)
        {
            colorPreview.color = currentColor; // Update color preview UI with the current color
        }

        // Ensure the FlexibleColorPicker shows the current color when opening the panel
        if (fcp != null && fcp.color != currentColor)
        {
            fcp.color = currentColor; // Set the color picker to the current color if it's different
        }
    }

    public void ClosePanel()
    {
        settingsPanel.SetActive(false);
    }
}