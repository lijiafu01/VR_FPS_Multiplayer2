using System.Collections.Generic;
using UnityEngine;
using Fusion;
namespace multiplayerMode
{
    public enum Weapon
    {
        Pistol,
        Bow,
        Card
    }
    public class WeaponManager : NetworkBehaviour
    {
        public NetworkTransform rightHand;
        public NetworkTransform leftHand;
        public Weapon CurrenWeapon;
        // Các mảng GameObject cho từng loại vũ khí
        public GameObject[] Pistol;
        public GameObject[] Bow;
        public GameObject[] Card;
        // Dictionary lưu trữ các loại vũ khí
        private Dictionary<Weapon, GameObject[]> weaponDict;
        //private string weaponName;
        [Networked(OnChanged = nameof(OnWeaponNameChanged))]
        public string weaponName { get; set; }
        private static void OnWeaponNameChanged(Changed<WeaponManager> changed)
        {
            // Gọi hàm không tĩnh để xử lý logic
            changed.Behaviour.OnWeaponNameChanged();
        }
        private void OnWeaponNameChanged()
        {
            if (weaponName == "Pistol")
            {
                SwitchWeapon(Weapon.Pistol);
            }
            else if (weaponName == "Bow")
            {
                SwitchWeapon(Weapon.Bow);
            }
            else if (weaponName == "Card")
            {
                SwitchWeapon(Weapon.Card);
            }
        }
        private void Awake()
        {
            weaponDict = new Dictionary<Weapon, GameObject[]>();
            // Thêm các vũ khí vào Dictionary với key là loại vũ khí
            weaponDict.Add(Weapon.Pistol, Pistol);
            weaponDict.Add(Weapon.Bow, Bow);
            weaponDict.Add(Weapon.Card, Card);
        }
        public override void Spawned()
        {
            if (weaponDict == null)
            {
                weaponDict = new Dictionary<Weapon, GameObject[]>();
                weaponDict.Add(Weapon.Pistol, Pistol);
                weaponDict.Add(Weapon.Bow, Bow);
                weaponDict.Add(Weapon.Card, Card);
            }
            if (weaponName != "")
            {
                if (weaponName == "Pistol")
                {
                    SwitchWeapon(Weapon.Pistol);
                }
                else if (weaponName == "Bow")
                {
                    SwitchWeapon(Weapon.Bow);
                }
                else if (weaponName == "Card")
                {
                    SwitchWeapon(Weapon.Card);
                }
            }
            else
            {
                if(Object.HasInputAuthority)
                {
                    if (UserEquipmentData.Instance.CurrentModelId == "Police")
                    {
                        weaponName = "Pistol";
                    }
                    else if (UserEquipmentData.Instance.CurrentModelId == "Angel")
                    {
                        weaponName = "Bow";
                    }
                    else if (UserEquipmentData.Instance.CurrentModelId == "Mage")
                    {
                        weaponName = "Card";
                    }
                }
            }
        }
        private void DeactivateAllWeapons()
        {
            if (weaponDict == null)
            {
                Debug.LogError("DeactivateAllWeapons: weaponDict is null.");
                return;
            }
            foreach (var weaponArray in weaponDict.Values)
            {
                if (weaponArray == null)
                {
                    Debug.LogWarning("DeactivateAllWeapons: Found a null weaponArray in weaponDict.");
                    continue;
                }
                foreach (var weaponObject in weaponArray)
                {
                    if (weaponObject != null)
                    {
                        weaponObject.SetActive(false); // Tắt đối tượng
                    }
                    else
                    {
                        Debug.LogWarning("DeactivateAllWeapons: Found a null weaponObject in weaponArray.");
                    }
                }
            }
        }
        // Phương thức để kích hoạt vũ khí hiện tại
        private void ActivateWeapon(Weapon currentWeapon)
        {
            if (weaponDict.ContainsKey(currentWeapon))
            {
                int index = 0;
                // Bật các đối tượng của vũ khí hiện tại
                foreach (var weaponObject in weaponDict[currentWeapon])
                {
                    if (weaponObject != null) // Kiểm tra nếu object không null
                    {
                        weaponObject.SetActive(true); // Bật đối tượng
                        if (index == 0)
                        {
                            leftHand.InterpolationTarget = weaponObject.transform;
                        }
                        else if(index == 1)
                        {
                            rightHand.InterpolationTarget = weaponObject.transform;
                        }
                        index++;
                    }
                }
            }
            else
            {
                Debug.LogError($"CurrentWeapon {currentWeapon} not found in the dictionary.");
            }
        }
        // Phương thức để chuyển đổi vũ khí
        private void SwitchWeapon(Weapon newWeapon)
        {
            PlayerController playerController = GetComponent<PlayerController>();
            playerController.CurrentWeapon = newWeapon;
            DeactivateAllWeapons();
            ActivateWeapon(newWeapon);
        }
    }
}
