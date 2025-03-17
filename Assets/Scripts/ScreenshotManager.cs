using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class ScreenshotManager : MonoBehaviour
{
    public string screenshotFolder = "Screenshots"; // Folder to save screenshots
    public Image flashEffect; // Optional UI flash effect
    public Camera screenshotCamera; // Assign the Main Camera or a dedicated camera for screenshots

    private void Start()
    {
        // Create screenshot folder if it doesn't exist
        string folderPath = Path.Combine(Application.persistentDataPath, screenshotFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    public void TakeScreenshot()
    {
        StartCoroutine(CaptureBlackAndWhiteScreenshot());
    }

    private IEnumerator CaptureBlackAndWhiteScreenshot()
    {
        yield return new WaitForEndOfFrame(); // Wait for frame to render

        // Create a RenderTexture to capture the screen
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenshotCamera.targetTexture = renderTexture;
        screenshotCamera.Render(); // Render the camera view into texture

        // Read pixels from RenderTexture
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // Cleanup
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Convert to Grayscale
        ConvertToGrayscale(screenshot);

        // Save screenshot
        SaveScreenshot(screenshot);
    }

    private void ConvertToGrayscale(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            float grayscale = pixels[i].grayscale; // Calculate grayscale value
            pixels[i] = new Color(grayscale, grayscale, grayscale); // Apply grayscale color
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    private void SaveScreenshot(Texture2D screenshot)
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = "Screenshot_" + timestamp + "_BW.png";
        string filePath = Path.Combine(Application.persistentDataPath, screenshotFolder, filename);

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Black & White Screenshot saved to: " + filePath);

        // Optional: Show a flash effect
        if (flashEffect != null)
        {
            StartCoroutine(FlashScreen());
        }

        // Destroy the texture to free memory
        Destroy(screenshot);
    }

    private IEnumerator FlashScreen()
    {
        flashEffect.gameObject.SetActive(true);
        flashEffect.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.2f);
        flashEffect.gameObject.SetActive(false);
    }
}
