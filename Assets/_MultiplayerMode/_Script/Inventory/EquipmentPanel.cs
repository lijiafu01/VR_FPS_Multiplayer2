using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _noticeText;
    public InventoryItemPanel inventoryItemPanel;
    private string _currentHero;
    
    [SerializeField] private TextMeshProUGUI _heroText;
    [SerializeField] private TextMeshProUGUI currentAttack;
    [SerializeField] private TextMeshProUGUI currentHp;
    [SerializeField] private TextMeshProUGUI currentMoveSpeed;
    [SerializeField] private TextMeshProUGUI attackLevel;
    [SerializeField] private TextMeshProUGUI hpLevel;
    [SerializeField] private TextMeshProUGUI moveSpeedLevel;
    [SerializeField] private TextMeshProUGUI attackUpdateText;
    [SerializeField] private TextMeshProUGUI hpUpdateText;
    [SerializeField] private TextMeshProUGUI moveSpeedUpdateText;

    [SerializeField] private TextMeshProUGUI _attackIncreaseText;
    [SerializeField] private TextMeshProUGUI _hpIncreaseText;
    [SerializeField] private TextMeshProUGUI _moveSpeedIncreaseText;

    [SerializeField] private TextMeshProUGUI _attackRate;
    [SerializeField] private TextMeshProUGUI _hpRate;
    [SerializeField] private TextMeshProUGUI _moveSpeedRate;


    private const int _hpIncrease = 20;
    private const int _attackIncrease = 5;
    private const int _moveSpeedIncrease = 2;

    private AttributeData _currentHeroData;
    private void OnEnable ()
    {
        _currentHero = UserEquipmentData.Instance.CurrentModelId;
        _heroText.text = _currentHero;
        SetUpHeroData();
        UpdateEquipmentPanel();

    }
    void SetUpHeroData()
    {
        if (PlayFabManager.Instance.UserData.PlayerAttributes.TryGetValue(_currentHero, out AttributeData heroAttributes))
        {
            _currentHeroData = heroAttributes;
            
        }
        
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

    public void UpdateEquipmentPanel()
    {
        _currentHero = UserEquipmentData.Instance.CurrentModelId;
        _heroText.text = _currentHero;
        SetUpHeroData();
        _attackIncreaseText.text = $"Attack +{_attackIncrease}";
        _hpIncreaseText.text = $"Hp +{_hpIncrease}"; 
        _moveSpeedIncreaseText.text = $"MoveSpeed +{_moveSpeedIncrease}";
        _attackRate.text = $"{((10 - _currentHeroData.Attack_Level) * 10)}%";
        _hpRate.text = $"{((10 - _currentHeroData.HP_Level) * 10)}%";
        _moveSpeedRate.text = $"{((10 - _currentHeroData.MoveSpeed_Level) * 10)}%";


        currentHp.text = "HP: "+ (_currentHeroData.originHP + (_currentHeroData.HP_Level * _hpIncrease)).ToString();
        currentMoveSpeed.text = "MoveSpeed: "+ (_currentHeroData.originMoveSpeed + (_currentHeroData.MoveSpeed_Level * _moveSpeedIncrease)).ToString();
        currentAttack.text ="Attack: "+ (_currentHeroData.originAttack + (_currentHeroData.Attack_Level * _attackIncrease)).ToString();

        attackLevel.text = _currentHeroData.Attack_Level.ToString();
        hpLevel.text = _currentHeroData.HP_Level.ToString();
        moveSpeedLevel.text = _currentHeroData.MoveSpeed_Level.ToString();

        attackUpdateText.text = $"{_currentHeroData.Attack_Level} -> {_currentHeroData.Attack_Level + 1}";
        hpUpdateText.text = $"{_currentHeroData.HP_Level} -> {_currentHeroData.HP_Level + 1}";
        moveSpeedUpdateText.text = $"{_currentHeroData.MoveSpeed_Level} -> {_currentHeroData.MoveSpeed_Level + 1}";
    }
    public void UpgradeAttack()
    {
        int currentQuantity = UserEquipmentData.Instance.GetItemQuantity(ItemType.Ruby, "Amethyst");

        if (currentQuantity > 0)
        {
            UserEquipmentData.Instance.SubtractItemQuantity(ItemType.Ruby, "Amethyst", 1);

            int successRate = (10 - _currentHeroData.Attack_Level) * 10;

            // Tạo một số ngẫu nhiên từ 0 đến 100
            int randomValue = Random.Range(0, 100);

            if (randomValue <= successRate)
            {
                _currentHeroData.Attack_Level += 1;
                DisplayText("Upgrade successful!"); 
            }
            else
            {
                DisplayText("Upgrade failed!");
            }

            PlayFabManager.Instance.UserData.UpgradePlayerAttributeData(_currentHero, _currentHeroData);
            inventoryItemPanel.PopulateInventory();
            UpdateEquipmentPanel();
            
        }
        else
        {
            DisplayText("Not enough items!");

        }
    }


    public void UpgradeHP()
    {
        int currentQuantity = UserEquipmentData.Instance.GetItemQuantity(ItemType.Ruby, "Emerald");

        if (currentQuantity > 0)
        {
            UserEquipmentData.Instance.SubtractItemQuantity(ItemType.Ruby, "Emerald", 1);

            int successRate = (10 - _currentHeroData.HP_Level) * 10;

            // Tạo một số ngẫu nhiên từ 0 đến 100
            int randomValue = Random.Range(0, 100);

            if (randomValue <= successRate)
            {
                DisplayText("Upgrade successful!");
                _currentHeroData.HP_Level += 1;

            }
            else
            {
                DisplayText("Upgrade failed!");
            }
            PlayFabManager.Instance.UserData.UpgradePlayerAttributeData(_currentHero, _currentHeroData);
            UpdateEquipmentPanel();
            inventoryItemPanel.PopulateInventory();

        }
        else
        {
            DisplayText("Not enough items!");
        }
    }

    public void UpgradeMoveSpeed()
    {
        int currentQuantity = UserEquipmentData.Instance.GetItemQuantity(ItemType.Ruby, "Sapphire");
        if (currentQuantity > 0)
        {
            UserEquipmentData.Instance.SubtractItemQuantity(ItemType.Ruby, "Sapphire", 1);

            int successRate = (10 - _currentHeroData.MoveSpeed_Level) * 10;

            // Tạo một số ngẫu nhiên từ 0 đến 100
            int randomValue = Random.Range(0, 100);

            if (randomValue <= successRate)
            {
                DisplayText("Upgrade successful!");
                _currentHeroData.MoveSpeed_Level += 1;


            }
            else
            {
                DisplayText("Upgrade failed!");
            }
            PlayFabManager.Instance.UserData.UpgradePlayerAttributeData(_currentHero, _currentHeroData);
            UpdateEquipmentPanel();
            inventoryItemPanel.PopulateInventory();

        }
        else
        {
            DisplayText("Not enough items!");
        }
    }
}
