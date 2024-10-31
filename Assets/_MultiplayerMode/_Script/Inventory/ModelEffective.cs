using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ModelEffective : MonoBehaviour, IModelObserver
{
    private UserEquipmentData modelObserver;

    private void OnEnable()
    {
        modelObserver = UserEquipmentData.Instance;
        if (modelObserver != null)
        {
            modelObserver.AddObserver(this); // Đăng ký lớp này làm observer
        }
        OnModelIdChanged(UserEquipmentData.Instance.CurrentModelId);
    }

    private void OnDisable()
    {
        if (modelObserver != null)
        {
            modelObserver.RemoveObserver(this); // Hủy đăng ký khi đối tượng bị vô hiệu hóa
        }
    }

    // Phương thức sẽ được gọi khi CurrentModelId thay đổi
    public void OnModelIdChanged(string newModelId)
    {
        // Lấy tất cả các đối tượng con của đối tượng hiện tại
        foreach (Transform child in transform)
        {
            // Tắt tất cả các đối tượng con
            child.gameObject.SetActive(false);

            // Nếu tên của đối tượng con trùng với newModelId, bật lại đối tượng con đó
            if (child.name == newModelId)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public Vector3 rotationSpeed = new Vector3(0, 100, 0);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
