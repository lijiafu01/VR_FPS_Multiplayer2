using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Item
{
    public string ItemId;
    public ItemType Type;
    public int Quantity;

    public Item(string itemId, ItemType type, int quantity = 1)
    {
        ItemId = itemId;
        Type = type;
        Quantity = quantity;
    }
}
public enum ItemType
{
    PlayerModel,
    Weapon,
    Armor,
    Ruby
    // Thêm các loại item khác
}
public class UserEquipmentData : MonoBehaviour
{
    private string _currentModelId;
    public static UserEquipmentData Instance;
    public Dictionary<ItemType, List<Item>> OwnedItems = new Dictionary<ItemType, List<Item>>();
    public bool DataLoaded = false;
    public string CurrentModelId
    {
        get => _currentModelId;
        set
        {
            if (_currentModelId != value)
            {
                _currentModelId = value;
                NotifyObservers(_currentModelId); // Thông báo đến các observer khi giá trị thay đổi
            }
        }
    }
    // Danh sách các observer (các lớp kế thừa `IModelObserver`)
    private List<IModelObserver> observers = new List<IModelObserver>();
    // Phương thức để thêm observer
    public void AddObserver(IModelObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }
    // Phương thức để loại bỏ observer
    public void RemoveObserver(IModelObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }
    // Thông báo đến tất cả các observer
    private void NotifyObservers(string newModelId)
    {
        foreach (var observer in observers)
        {
            observer.OnModelIdChanged(newModelId);
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        DataLoaded = false;
        LoadUserEquipmentDataFromPlayFab();
    }
    public void EnsureDefaultModel()
    {
        if (!OwnedItems.ContainsKey(ItemType.PlayerModel) || OwnedItems[ItemType.PlayerModel].Count == 0)
        {
            // Thêm mô hình "Police" mặc định
            Item defaultModel = new Item( "Police", ItemType.PlayerModel );
            AddItem(defaultModel);
            // Đặt mô hình hiện tại là "Police"
            CurrentModelId = "Police";
            SaveUserEquipmentDataToPlayFab();
        }
    }
    public void AddItem(Item newItem)
    {
        if (!DataLoaded)
        {
            Debug.LogWarning("dev5_Data not loaded yet, cannot add item.");
            return;
        }
        if (!OwnedItems.ContainsKey(newItem.Type))
        {
            OwnedItems[newItem.Type] = new List<Item>();
        }
        // Tìm item trong danh sách dựa trên ItemId
        var existingItem = OwnedItems[newItem.Type].Find(item => item.ItemId == newItem.ItemId);
        if (existingItem != null)
        {
            // Tăng số lượng nếu item đã tồn tại
            existingItem.Quantity += newItem.Quantity;
        }
        else
        {
            // Thêm item mới với số lượng ban đầu
            OwnedItems[newItem.Type].Add(newItem);
            Debug.Log($"Added new item {newItem.ItemId} of type {newItem.Type} with quantity {newItem.Quantity}");
        }
        SaveUserEquipmentDataToPlayFab();
    }
    private void SaveUserEquipmentDataToPlayFab()
    {
        string jsonData = JsonConvert.SerializeObject(OwnedItems);
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { "UserEquipmentData", jsonData },
            { "CurrentModelId", CurrentModelId }
        }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendFailure);
    }
    public void SetCurrentModel(string modelId)
    {
        if (!OwnedItems.ContainsKey(ItemType.PlayerModel) || !OwnedItems[ItemType.PlayerModel].Exists(item => item.ItemId == modelId))
        {
            Debug.LogWarning("dev5_Model not owned: " + modelId);
            return;
        }
        CurrentModelId = modelId;
        SaveUserEquipmentDataToPlayFab();
    }
    private void OnDataSendSuccess(UpdateUserDataResult result)
    {
    }
    private void OnDataSendFailure(PlayFabError error)
    {
        Debug.LogError("dev5_Error saving data to PlayFab: " + error.GenerateErrorReport());
    }
    public void LoadUserEquipmentDataFromPlayFab()
    {
        var request = new GetUserDataRequest
        {
            Keys = new List<string> { "UserEquipmentData", "CurrentModelId" }
        };

        PlayFabClientAPI.GetUserData(request, OnDataReceiveSuccess, OnDataReceiveFailure);
    }
    private void OnDataReceiveSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("UserEquipmentData"))
        {
            string jsonData = result.Data["UserEquipmentData"].Value;
            OwnedItems = JsonConvert.DeserializeObject<Dictionary<ItemType, List<Item>>>(jsonData);
            if (OwnedItems.ContainsKey(ItemType.PlayerModel))
            {
                var models = OwnedItems[ItemType.PlayerModel];
                // Luôn cố gắng lấy CurrentModelId từ dữ liệu nếu nó tồn tại
                if (result.Data.ContainsKey("CurrentModelId"))
                {
                    CurrentModelId = result.Data["CurrentModelId"].Value;
                }
                else
                {
                    // Nếu không có CurrentModelId trong dữ liệu, đặt giá trị mặc định
                    if (models.Exists(item => item.ItemId == "Police"))
                    {
                        CurrentModelId = "Police";
                    }
                    else if (models.Count > 0)
                    {
                        CurrentModelId = models[0].ItemId;
                    }
                    else
                    {
                        // Nếu người chơi không sở hữu mô hình nào, thêm mô hình "Police" mặc định
                        Item defaultModel = new Item ("Police", ItemType.PlayerModel);
                        OwnedItems[ItemType.PlayerModel].Add(defaultModel);
                        CurrentModelId = "Police";
                        SaveUserEquipmentDataToPlayFab();
                    }
                }
            }
            else
            {
                // Nếu không có mô hình nào, thêm mô hình "Police" mặc định
                OwnedItems[ItemType.PlayerModel] = new List<Item>();
                Item defaultModel = new Item("Police", ItemType.PlayerModel);
                OwnedItems[ItemType.PlayerModel].Add(defaultModel);
                CurrentModelId = "Police";
                SaveUserEquipmentDataToPlayFab();
            }
            /*// In ra danh sách các item đã tải
            foreach (var kvp in OwnedItems)
            {
                foreach (var item in kvp.Value)
                {
                    Debug.Log("dev5_Owned Item: " + item.ItemId + " of type " + item.Type);
                }
            }*/
        }
        else
        {
            OwnedItems = new Dictionary<ItemType, List<Item>>();
            // Thêm mô hình "Police" mặc định
            Item defaultModel = new Item("Police", ItemType.PlayerModel);
            OwnedItems[ItemType.PlayerModel] = new List<Item> { defaultModel };
            CurrentModelId = "Police";
            SaveUserEquipmentDataToPlayFab();
        }
        DataLoaded = true;
        EnsureDefaultModel();
    }
    private void OnDataReceiveFailure(PlayFabError error)
    {
        Debug.LogError("dev5_Error loading data from PlayFab: " + error.GenerateErrorReport());
        DataLoaded = true; // Để tránh bị treo nếu tải dữ liệu thất bại
    }
    private void Start()
    {
        Initialize();
        StartCoroutine(WaitUntilDataLoaded());
    }
    private IEnumerator WaitUntilDataLoaded()
    {
        while (!DataLoaded)
        {
            yield return null;
        }
       /* // In ra danh sách các Item sở hữu
        PrintOwnedItems();*/
    }
    // Hàm lấy số lượng vật phẩm theo ItemType và tên vật phẩm
    public int GetItemQuantity(ItemType type, string itemId)
    {
        if (!DataLoaded)
        {
            Debug.LogWarning("Data not loaded yet, cannot get item quantity.");
            return 0;
        }
        if (OwnedItems.ContainsKey(type))
        {
            // Tìm item trong danh sách dựa trên ItemId
            var item = OwnedItems[type].Find(i => i.ItemId == itemId);
            if (item != null)
            {
                return item.Quantity;
            }
        }
        Debug.LogWarning($"Item {itemId} of type {type} not found in OwnedItems.");
        return 0;
    }

    public void SubtractItemQuantity(ItemType type, string itemId, int quantityToSubtract)
    {
        if (!DataLoaded)
        {
            Debug.LogWarning("Data not loaded yet, cannot subtract item quantity.");
            return;
        }
        if (quantityToSubtract <= 0)
        {
            Debug.LogWarning("Quantity to subtract must be greater than zero.");
            return;
        }
        if (!OwnedItems.ContainsKey(type))
        {
            Debug.LogWarning($"Item type {type} not found in OwnedItems.");
            return;
        }
        // Tìm item trong danh sách dựa trên ItemId
        var itemList = OwnedItems[type];
        var existingItem = itemList.Find(item => item.ItemId == itemId);
        if (existingItem != null)
        {
            if (existingItem.Quantity > quantityToSubtract)
            {
                existingItem.Quantity -= quantityToSubtract;
            }
            else
            {
                // Số lượng cần trừ lớn hơn hoặc bằng số lượng hiện có
                itemList.Remove(existingItem);
            }
            // Lưu dữ liệu cập nhật lên PlayFab
            SaveUserEquipmentDataToPlayFab();
        }
        else
        {
            Debug.LogWarning($"Item {itemId} of type {type} not found in OwnedItems.");
        }
    }
   /* public void PrintOwnedItems()
    {
        foreach (var kvp in OwnedItems)
        {
            foreach (var item in kvp.Value)
            {
                Debug.Log("dev5_ - Item ID danh sach item: " + item.ItemId);
            }
        }
    }*/
}
