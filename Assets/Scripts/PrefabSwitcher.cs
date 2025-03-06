using UnityEngine;
using UnityEngine.UI;

public class PrefabSwitcher : MonoBehaviour
{
    public GameObject[] prefabs;
    public Texture[] prefabTextures;
    private int currentPrefabIndex = 0;
    public placeDelete placeDeleteScript;
    public Button nextButton;
    public Button previousButton;
    public RawImage prefabTextureDisplay;

    void Start()
    {
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
        nextButton.onClick.AddListener(SwitchToNextPrefab);
        previousButton.onClick.AddListener(SwitchToPreviousPrefab);
    }

    void SwitchToNextPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex + 1) % prefabs.Length;
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
    }

    void SwitchToPreviousPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex - 1 + prefabs.Length) % prefabs.Length;
        placeDeleteScript.objectToPlace = prefabs[currentPrefabIndex];
        UpdatePrefabTexture();
    }

    void UpdatePrefabTexture()
    {
        if (prefabTextures.Length > 0 && prefabTextures.Length > currentPrefabIndex)
        {
            prefabTextureDisplay.texture = prefabTextures[currentPrefabIndex];
        }
    }
}
