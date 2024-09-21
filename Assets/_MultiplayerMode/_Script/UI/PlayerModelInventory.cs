using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerModelInventory : MonoBehaviour
{
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

    private void PopulateInventory()
    {
        // Xóa các item cũ trong Content
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        var models = UserEquipmentData.Instance.OwnedItems[ItemType.PlayerModel];

        foreach (var item in models)
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

            // Thiết lập nút UseButton
            Button useButton = itemObj.transform.Find("UseButton").GetComponent<Button>();
            TextMeshProUGUI buttonText = useButton.GetComponentInChildren<TextMeshProUGUI>();

            if (UserEquipmentData.Instance.CurrentModelId == item.ItemId)
            {
                // Mô hình đang được sử dụng
                useButton.interactable = false;
                ColorBlock colors = useButton.colors;
                colors.normalColor = Color.gray;
                useButton.colors = colors;
                buttonText.text = "Using";
            }
            else
            {
                // Mô hình chưa được sử dụng
                useButton.interactable = true;
                ColorBlock colors = useButton.colors;
                colors.normalColor = Color.green;
                useButton.colors = colors;
                buttonText.text = "Use";
            }

            // Thêm sự kiện khi bấm nút UseButton
            string modelId = item.ItemId; // Lưu lại biến cục bộ để tránh lỗi closure
            useButton.onClick.AddListener(() => OnUseButtonClicked(modelId));
        }
    }

    private void OnUseButtonClicked(string modelId)
    {
        UserEquipmentData.Instance.SetCurrentModel(modelId);
        PopulateInventory();
    }

    private Sprite LoadSpriteFromResources(string modelId)
    {
        // Giả sử bạn lưu hình ảnh trong thư mục "Resources/ModelImages"
        string path = "ModelImages/" + modelId;
        return Resources.Load<Sprite>(path);
    }
}
