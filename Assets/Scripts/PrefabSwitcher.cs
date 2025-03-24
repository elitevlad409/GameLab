using UnityEngine;
using UnityEngine.UI; // Add this for RawImage, Button, etc.

public class PrefabSwitcher : MonoBehaviour
{
    public GameObject[] prefabs;
    public Texture[] prefabTextures;
    private int currentPrefabIndex = 0;
    public placeDelete placeDeleteScript;
    public PrefabPreviewer previewer; // Reference to PrefabPreviewer
    public Button nextButton;
    public Button previousButton;
    public RawImage prefabTextureDisplay;

    void Start()
    {
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
        previewer.UpdatePreviewPrefab(); // Update preview on start
        nextButton.onClick.AddListener(SwitchToNextPrefab);
        previousButton.onClick.AddListener(SwitchToPreviousPrefab);
    }

    void SwitchToNextPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex + 1) % prefabs.Length;
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
        previewer.UpdatePreviewPrefab(); // Update preview when switching
    }

    void SwitchToPreviousPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex - 1 + prefabs.Length) % prefabs.Length;
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
        previewer.UpdatePreviewPrefab(); // Update preview when switching
    }

    void UpdatePrefabTexture()
    {
        if (prefabTextures.Length > 0 && prefabTextures.Length > currentPrefabIndex)
        {
            prefabTextureDisplay.texture = prefabTextures[currentPrefabIndex];
        }
    }
}