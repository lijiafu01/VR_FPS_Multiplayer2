using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TrainingVideo : MonoBehaviour
{
    public WeaponType Weapon;
    private VideoPlayer videoPlayer;

   
    public void Show()
    {
        if (Weapon == GameManager.Instance.playerChooseWeapon)
        {
            // Lấy VideoPlayer từ Plane
            videoPlayer = GetComponent<VideoPlayer>();
            // Bắt đầu phát video khi game bắt đầu
            videoPlayer.Play();
            float videoLength = (float)videoPlayer.clip.length;
            Invoke("CloseObject", videoLength);
        }
        
    }
    void CloseObject()
    {
        transform.parent.gameObject.SetActive(false);  
    }

}
