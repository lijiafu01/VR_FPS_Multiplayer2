using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Thêm thư viện TextMeshPro

public class ShopManager : MonoBehaviour
{
    public GameObject BuySuccessTab;
    public GameObject NoMoneyTab;
    [SerializeField]
    private GameObject _itemTemplate; // Template của item (Prefab)
    [SerializeField]
    private Transform _content; // Transform của Content trong ScrollView

    [SerializeField]
    private List<ShopItem> _shopItems; // Danh sách các item trong cửa hàng

    void Start()
    {
        PopulateShop();
    }

    private void PopulateShop()
    {
        foreach (ShopItem item in _shopItems)
        {
            // Tạo instance của itemTemplate
            GameObject itemObj = Instantiate(_itemTemplate, _content);
            itemObj.SetActive(true);
            // Thiết lập tên item
            TextMeshProUGUI itemNameText = itemObj.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>();
            itemNameText.text = item.itemName;

            // Thiết lập hình ảnh item
            Image itemImage = itemObj.transform.Find("ItemImage").GetComponent<Image>();
            itemImage.sprite = item.itemImage;

            // Thiết lập giá và sự kiện cho nút mua
            Button buyButton = itemObj.transform.Find("BuyButton").GetComponent<Button>();
            TextMeshProUGUI priceText = buyButton.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
            priceText.text = "$" + item.itemPrice;

            // Thêm sự kiện khi bấm nút mua
            buyButton.onClick.AddListener(() => OnBuyButtonClicked(item));
        }
    }

    private void OnBuyButtonClicked(ShopItem item)
    {
        int amountToSubtract = item.itemPrice;

        PlayFabManager.Instance.CurrencyManager.SubtractGoldCoin(amountToSubtract, (amountSubtracted) =>
        {
           

            if (amountSubtracted > 0)
            {
                BuySuccessTab.gameObject.SetActive(true);

                Item newItem = new Item();
                newItem.ItemId = item.itemName;
                newItem.Type = item.type;

                UserEquipmentData.Instance.AddItem(newItem);
                Debug.Log($"dev3_Đã trừ thành công {amountSubtracted} coin.");
                // Thực hiện các hành động tiếp theo, ví dụ: cập nhật giao diện người dùng
            }
            else
            {
                NoMoneyTab.gameObject.SetActive(true);
                Debug.Log("dev3_Không thể trừ coin. Người chơi không đủ tiền hoặc có lỗi xảy ra.");
                // Thông báo cho người chơi
            }
        });

        Debug.Log("Bạn đã mua: " + item.itemName + " với giá $" + item.itemPrice);
    }
}

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public Sprite itemImage;
    public int itemPrice;
    public ItemType type;
}
