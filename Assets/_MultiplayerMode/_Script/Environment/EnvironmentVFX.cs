using UnityEngine;
public class EnvironmentVFX : MonoBehaviour
{
    [SerializeField] private Material[] skyboxes; // Mảng các Skybox có thể gán từ Inspector
    private int currentSkyboxIndex = 0; // Chỉ số Skybox hiện tại
    public void ChangeSkyboxByIndex(int index)
    {
        if (index >= 0 && index < skyboxes.Length)
        {
            RenderSettings.skybox = skyboxes[index];
            currentSkyboxIndex = index;
        }
        else
        {
            Debug.LogWarning("Skybox index out of range.");
        }
    }
    public void NextSkybox()
    {
        currentSkyboxIndex = (currentSkyboxIndex + 1) % skyboxes.Length;
        RenderSettings.skybox = skyboxes[currentSkyboxIndex];
    }
    public void PreviousSkybox()
    {
        currentSkyboxIndex = (currentSkyboxIndex - 1 + skyboxes.Length) % skyboxes.Length;
        RenderSettings.skybox = skyboxes[currentSkyboxIndex];
    }
}
