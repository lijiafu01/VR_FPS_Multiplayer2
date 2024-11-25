using UnityEngine;
public class VFXSpeedController : MonoBehaviour
{
    [Tooltip("Hệ số tăng tốc độ chạy của VFX")]
    public float speedMultiplier = 1.0f;
    private ParticleSystem[] particleSystems;
    void Awake()
    {
        // Lấy tất cả các Particle System trong đối tượng hiện tại và các con cháu
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    void OnEnable()
    {
        // Điều chỉnh simulationSpeed của tất cả các Particle System
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            main.simulationSpeed = speedMultiplier;
        }
    }
    public void UpdateSpeed(float newSpeed)
    {
        speedMultiplier = newSpeed;
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            main.simulationSpeed = speedMultiplier;
        }
    }
}
