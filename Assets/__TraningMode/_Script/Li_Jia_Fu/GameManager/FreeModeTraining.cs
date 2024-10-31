using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeModeTraining : MonoBehaviour
{
    public GameObject[] enemies; // Danh sách các đối tượng kẻ thù có thể spawn
    public Transform spawnPosLeft; // Vị trí spawn bên trái
    public Transform spawnRight;   // Vị trí spawn bên phải

    private void OnEnable()
    {
        StartCoroutine(SpawnWaves()); // Bắt đầu các đợt spawn
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            // Đợi ngẫu nhiên từ 3 đến 5 giây giữa các đợt
            float waitTime = Random.Range(5f, 8f);
            yield return new WaitForSeconds(waitTime);

            // Số lượng đối tượng spawn trong đợt hiện tại (5 đến 10)
            int spawnCount = Random.Range(4, 7);

            for (int i = 0; i < spawnCount; i++)
            {
                SpawnRandomEnemy();
            }
        }
    }

    void SpawnRandomEnemy()
    {
        // Chọn ngẫu nhiên một vị trí spawn trong phạm vi giữa spawnPosLeft và spawnRight
        float spawnX = Random.Range(spawnPosLeft.position.x, spawnRight.position.x);
        float spawnY = Random.Range(spawnPosLeft.position.y, spawnRight.position.y);
        float spawnZ = Random.Range(spawnPosLeft.position.z, spawnRight.position.z);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        // Chọn ngẫu nhiên một kẻ thù từ danh sách enemies
        GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];

        // Tạo đối tượng kẻ thù tại vị trí spawn ngẫu nhiên
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
