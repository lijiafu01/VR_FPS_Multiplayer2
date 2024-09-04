using UnityEngine;
using Fusion;

public class HealthPickup : NetworkBehaviour
{
    private int health = 30;
    [Networked(OnChanged = nameof(OnStateChanged))]
    private bool IsActive { get; set; } = true;

    public float respawnTime = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("dev15_haha");
        if (IsActive && other.CompareTag("Player"))
        {
            Debug.Log("dev15_2haha");

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.IncreaseHealth(health); // Tăng HP cho người chơi
                DespawnPickup(); // Ẩn cục HP và bắt đầu thời gian chờ để respawn
                Debug.Log("dev15_3haha");

            }
        }
    }

    private void DespawnPickup()
    {
        IsActive = false; // Cập nhật trạng thái để đồng bộ với các client khác
        Invoke(nameof(RespawnPickup), respawnTime);
    }

    private void RespawnPickup()
    {
        IsActive = true; // Cập nhật trạng thái để đồng bộ với các client khác
    }

    private static void OnStateChanged(Changed<HealthPickup> changed)
    {
        bool isActive = changed.Behaviour.IsActive;
        changed.Behaviour.gameObject.SetActive(isActive); // Bật/tắt cục HP dựa trên trạng thái đồng bộ
    }
}
