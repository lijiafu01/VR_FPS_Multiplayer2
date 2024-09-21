using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class Item
{
    public string ItemId;
    public ItemType Type;
    // Thêm thuộc tính khác nếu cần
}

public enum ItemType
{
    PlayerModel,
    Weapon,
    Armor,
    // Thêm các loại item khác
}

public class UserEquipmentData : MonoBehaviour
{
    public static UserEquipmentData Instance;

    public Dictionary<ItemType, List<Item>> OwnedItems = new Dictionary<ItemType, List<Item>>();

    private bool dataLoaded = false;

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
        dataLoaded = false;
        LoadUserEquipmentDataFromPlayFab();
    }

    public void AddItem(Item newItem)
    {
        if (!dataLoaded)
        {
            Debug.LogWarning("dev5_Data not loaded yet, cannot add item.");
            return;
        }

        if (!OwnedItems.ContainsKey(newItem.Type))
        {
            OwnedItems[newItem.Type] = new List<Item>();
        }

        if (!OwnedItems[newItem.Type].Exists(item => item.ItemId == newItem.ItemId))
        {
            OwnedItems[newItem.Type].Add(newItem);
            SaveUserEquipmentDataToPlayFab();

            Debug.Log("dev5_Item added: " + newItem.ItemId + " of type " + newItem.Type);
        }
        else
        {
            Debug.Log("dev5_Item already exists: " + newItem.ItemId);
        }
    }

    private void SaveUserEquipmentDataToPlayFab()
    {
        string jsonData = JsonConvert.SerializeObject(OwnedItems);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "UserEquipmentData", jsonData }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendFailure);
    }

    private void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("dev5_Data saved to PlayFab successfully.");
    }

    private void OnDataSendFailure(PlayFabError error)
    {
        Debug.LogError("dev5_Error saving data to PlayFab: " + error.GenerateErrorReport());
    }

    public void LoadUserEquipmentDataFromPlayFab()
    {
        var request = new GetUserDataRequest
        {
            Keys = new List<string> { "UserEquipmentData" }
        };

        PlayFabClientAPI.GetUserData(request, OnDataReceiveSuccess, OnDataReceiveFailure);
    }

    private void OnDataReceiveSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("UserEquipmentData"))
        {
            string jsonData = result.Data["UserEquipmentData"].Value;
            OwnedItems = JsonConvert.DeserializeObject<Dictionary<ItemType, List<Item>>>(jsonData);
            Debug.Log("dev5_Data loaded from PlayFab successfully.");

            // In ra danh sách các item đã tải
            foreach (var kvp in OwnedItems)
            {
                foreach (var item in kvp.Value)
                {
                    Debug.Log("dev5_Owned Item: " + item.ItemId + " of type " + item.Type);
                }
            }
        }
        else
        {
            Debug.Log("dev5_No equipment data found on PlayFab, initializing new data.");
            OwnedItems = new Dictionary<ItemType, List<Item>>();
        }

        dataLoaded = true;
    }

    private void OnDataReceiveFailure(PlayFabError error)
    {
        Debug.LogError("dev5_Error loading data from PlayFab: " + error.GenerateErrorReport());
        dataLoaded = true; // Để tránh bị treo nếu tải dữ liệu thất bại
    }

    private void Start()
    {
        Initialize();
       
        StartCoroutine(WaitUntilDataLoaded());
    }

    private IEnumerator WaitUntilDataLoaded()
    {
        while (!dataLoaded)
        {
            yield return null;
        }


        // In ra danh sách các Item sở hữu
        PrintOwnedItems();
    }

    /*private void AddTestItems()
    {
        Item newItem1 = new Item();
        newItem1.ItemId = "Item1";
        newItem1.Type = ItemType.PlayerModel;
        AddItem(newItem1);

        Item newItem2 = new Item();
        newItem2.ItemId = "Item2";
        newItem2.Type = ItemType.Weapon;
        AddItem(newItem2);

        Item newItem3 = new Item();
        newItem3.ItemId = "Item3";
        newItem3.Type = ItemType.Armor;
        AddItem(newItem3);
    }*/

    public void PrintOwnedItems()
    {
        Debug.Log("dev5_Printing owned items:");
        foreach (var kvp in OwnedItems)
        {
            Debug.Log("dev5_Item Type: " + kvp.Key);
            foreach (var item in kvp.Value)
            {
                Debug.Log("dev5_ - Item ID danh sach item: " + item.ItemId);
            }
        }
    }
}
