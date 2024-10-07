using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using multiplayerMode;
namespace multiplayerMode
{
    public enum Weapon
    {
        Pistol,
        Bow
    }

    public class WeaponManager : MonoBehaviour
    {
        public NetworkTransform rightHand;
        public NetworkTransform leftHand;
        public Weapon CurrenWeapon;

        // Các mảng GameObject cho từng loại vũ khí
        public GameObject[] Pistol;
        public GameObject[] Bow;

        // Dictionary lưu trữ các loại vũ khí
        private Dictionary<Weapon, GameObject[]> weaponDict;

        private void Start()
        {

            // Khởi tạo Dictionary
            weaponDict = new Dictionary<Weapon, GameObject[]>();

            // Thêm các vũ khí vào Dictionary với key là loại vũ khí
            weaponDict.Add(Weapon.Pistol, Pistol);
            weaponDict.Add(Weapon.Bow, Bow);

            // Tắt tất cả các vũ khí khi khởi đầu
            DeactivateAllWeapons();

            // Kích hoạt vũ khí hiện tại
            ActivateWeapon(CurrenWeapon);
           

        }
       /* void SwitchWeaponWrapper()
        {
            // Gọi phương thức SwitchWeapon và truyền tham số
            SwitchWeapon(CurrentWeapon.Pistol);
        }
        void SwitchWeaponWrapper2()
        {
            // Gọi phương thức SwitchWeapon và truyền tham số
            SwitchWeapon(CurrentWeapon.Bow);
        }*/
        // Phương thức để tắt tất cả các vũ khí
        private void DeactivateAllWeapons()
        {
            foreach (var weaponArray in weaponDict.Values)
            {
                foreach (var weaponObject in weaponArray)
                {
                    if (weaponObject != null) // Kiểm tra nếu object không null
                    {
                        weaponObject.SetActive(false); // Tắt đối tượng
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
        public void SwitchWeapon(Weapon newWeapon)
        {
            // Tắt vũ khí hiện tại
            DeactivateAllWeapons();

            // Cập nhật vũ khí và kích hoạt vũ khí mới
            CurrenWeapon = newWeapon;
            ActivateWeapon(CurrenWeapon);
            PlayerController player = GetComponentInParent<PlayerController>();
            player.CurrentWeapon = CurrenWeapon;

        }
    }
}
