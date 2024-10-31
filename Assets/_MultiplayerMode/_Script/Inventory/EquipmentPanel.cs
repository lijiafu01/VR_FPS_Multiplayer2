using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentPanel : MonoBehaviour
{
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

    private const int _hpIncrease = 120;
    private const int _attackIncrease = 50;
    private const int _moveSpeedIncrease = 3;

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
    public void UpdateEquipmentPanel()
    {
        _currentHero = UserEquipmentData.Instance.CurrentModelId;
        _heroText.text = _currentHero;
        SetUpHeroData();
        _attackIncreaseText.text = $"Attack +{_attackIncrease}";
        _hpIncreaseText.text = $"Hp +{_hpIncrease}"; 
        _moveSpeedIncreaseText.text = $"MoveSpeed +{_moveSpeedIncrease}";

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
        _currentHeroData.Attack_Level += 1;
        PlayFabManager.Instance.UserData.UpgradePlayerAttributeData(_currentHero,_currentHeroData);
        UpdateEquipmentPanel();
    }
    
    public void UpgradeHP()
    {
        _currentHeroData.HP_Level += 1;
        PlayFabManager.Instance.UserData.UpgradePlayerAttributeData(_currentHero, _currentHeroData);
        UpdateEquipmentPanel();
    }

    public void UpgradeMoveSpeed()
    {
        _currentHeroData.MoveSpeed_Level += 1;
        PlayFabManager.Instance.UserData.UpgradePlayerAttributeData(_currentHero, _currentHeroData);
        UpdateEquipmentPanel();
    }
}
