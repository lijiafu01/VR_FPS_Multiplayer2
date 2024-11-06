using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPanel : MonoBehaviour
{
    public ItemUI template; // Template để tạo các item phần thưởng

    public void SpawnRewardItem(ItemRewardType itemRewardType, int quantity)
    {
        // Tạo một bản sao của template và thêm vào RewardPanel
        ItemUI rewardItem = Instantiate(template, transform);

        // Bật template lên (trường hợp ban đầu đang bị tắt)
        rewardItem.gameObject.SetActive(true);

        // Gọi hàm Init để thiết lập loại item và số lượng
        rewardItem.Init(itemRewardType, quantity);
    }
    private void OnEnable()
    {
        // Xóa tất cả các đối tượng con khi RewardPanel bị tắt
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
