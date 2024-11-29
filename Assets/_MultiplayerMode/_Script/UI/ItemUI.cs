using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum ItemRewardType
{
    Coin,
    Amethyst,
    Emerald,
    Sapphire
}
public class ItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quantityText; // Hiển thị số lượng
    [SerializeField] private UnityEngine.UI.Image itemImage; // Hình ảnh UI hiển thị item
    [System.Serializable]
    public class ItemSprite
    {
        public ItemRewardType itemType;
        public Sprite sprite;
    }
    [Header("Item Sprites")]
    [SerializeField] private List<ItemSprite> itemSprites; // Danh sách để thiết lập item và hình ảnh tương ứng
    public void Init(ItemRewardType itemType, int quantity)
    {
        // Tìm và gán hình ảnh dựa trên loại item
        ItemSprite foundItem = itemSprites.Find(item => item.itemType == itemType);
        if (foundItem != null && foundItem.sprite != null)
        {
            itemImage.sprite = foundItem.sprite;
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy hình ảnh cho loại item: {itemType}");
        }
        // Cập nhật số lượng hiển thị
        quantityText.text ="+"+ quantity.ToString();
    }
}
