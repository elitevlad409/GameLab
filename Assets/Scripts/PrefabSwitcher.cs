using UnityEngine;
using UnityEngine.UI;

public class PrefabSwitcher : MonoBehaviour
{
    public GameObject[] prefabs; // Array of prefabs to switch between
    public Texture[] prefabTextures; // Array of Textures for each prefab
    private int currentPrefabIndex = 0; // The index of the current prefab
    public placeDelete placeDeleteScript; // Reference to the placeDelete script
    public Button nextButton; // Reference to the Next Button
    public Button previousButton; // Reference to the Previous Button
    public RawImage prefabTextureDisplay; // Reference to the RawImage UI to display prefab textures

    void Start()
    {
        // Initialize the placeDelete script with the first prefab
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];

        // Set the initial prefab texture
        UpdatePrefabTexture();

        // Button listeners
        nextButton.onClick.AddListener(SwitchToNextPrefab);
        previousButton.onClick.AddListener(SwitchToPreviousPrefab);
    }

    void SwitchToNextPrefab()
    {
        // Move to the next prefab in the list, wrap around if needed
        currentPrefabIndex = (currentPrefabIndex + 1) % prefabs.Length;
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
    }

    void SwitchToPreviousPrefab()
    {
        // Move to the previous prefab in the list, wrap around if needed
        currentPrefabIndex = (currentPrefabIndex - 1 + prefabs.Length) % prefabs.Length;
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
    }

    void UpdatePrefabTexture()
    {
        // Make sure the array has the correct size before updating
        if (prefabTextures.Length > 0 && prefabTextures.Length > currentPrefabIndex)
        {
            // Set the texture of the RawImage based on the selected prefab index
            prefabTextureDisplay.texture = prefabTextures[currentPrefabIndex];
        }
    }
}
