using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryItemPanel : MonoBehaviour
{
    public TextMeshProUGUI _noticeText;
    [SerializeField]
    private GameObject _itemTemplate; // Template của item (Prefab)
    [SerializeField]
    private Transform _content; // Transform của Content trong ScrollView
    private void OnEnable()
    {
        StartCoroutine(InitializeInventory());
    }
    private IEnumerator InitializeInventory()
    {
        // Chờ đến khi dữ liệu đã được tải
        while (!UserEquipmentData.Instance.DataLoaded)
        {
            yield return null;
        }

        PopulateInventory();
    }
    public void PopulateInventory()
    {
        // Xóa các item cũ trong Content
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
        var models = UserEquipmentData.Instance.OwnedItems[ItemType.Ruby];
        foreach (var item in models)
        {
            for(int i = 0;i<item.Quantity;i++)
            {
                GameObject itemObj = Instantiate(_itemTemplate, _content);
                itemObj.SetActive(true);
                // Thiết lập tên item
                TextMeshProUGUI itemNameText = itemObj.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>();
                itemNameText.text = item.ItemId;
                // Thiết lập hình ảnh item
                Image itemImage = itemObj.transform.Find("ItemImage").GetComponent<Image>();
                Sprite modelSprite = LoadSpriteFromResources(item.ItemId);
                if (modelSprite != null)
                {
                    itemImage.sprite = modelSprite;
                }
                else
                {
                    Debug.LogWarning("dev5_Could not find image for model: " + item.ItemId);
                }
                Button SellButton = itemObj.transform.Find("SellButton").GetComponent<Button>();
                string modelId = item.ItemId; // Lưu lại biến cục bộ để tránh lỗi closure
                SellButton.onClick.AddListener(() => OnUseButtonClicked(modelId));
            }
        }
    }
    private void OnUseButtonClicked(string modelId)
    {
        int currentQuantity = UserEquipmentData.Instance.GetItemQuantity(ItemType.Ruby, modelId);
        if (currentQuantity > 0)
        {
            UserEquipmentData.Instance.SubtractItemQuantity(ItemType.Ruby, modelId, 1);
            int price = 1;
            PlayFabManager.Instance.CurrencyManager.AddCurrency("GC", price);
            string text = $"Sold, gold +{price}!";
            DisplayText(text);
        }
        PopulateInventory();
    }
    void DisplayText(string content)
    {
        _noticeText.gameObject.SetActive(true);
        _noticeText.text = content;
        CancelInvoke("TurnOfftext"); // Hủy Invoke trước đó nếu có
        Invoke("TurnOfftext", 2f);    // Đặt lại Invoke mới
    }
    void TurnOfftext()
    {
        _noticeText.gameObject.SetActive(false);
    }
    private Sprite LoadSpriteFromResources(string modelId)
    {
        // Giả sử bạn lưu hình ảnh trong thư mục "Resources/ModelImages"
        string path = "Items/" + modelId;
        return Resources.Load<Sprite>(path);
    }
}
