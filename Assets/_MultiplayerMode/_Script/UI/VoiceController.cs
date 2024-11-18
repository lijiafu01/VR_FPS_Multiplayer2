using UnityEngine;
using Photon.Voice.Unity;
using multiplayerMode;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VoiceController : MonoBehaviour
{
    private bool _isMute = false;
    private bool _isStop = false;
    public Image micImage;
    public Image volumImage;
    public void MuteAllPlayers()
    {
        Speaker[] speakers = FindObjectsOfType<Speaker>();

        foreach (Speaker speaker in speakers)
        {
            if (speaker != null)
            {
                speaker.enabled = false; // Tắt âm thanh
            }
        }
    }

    public void UnmuteAllPlayers()
    {
        Speaker[] speakers = FindObjectsOfType<Speaker>();

        foreach (Speaker speaker in speakers)
        {
            if (speaker != null)
            {
                speaker.enabled = true; // Bật âm thanh lại
            }
        }
    }
    public void MicBtn()
    {
        _isMute = !_isMute;
        UpdateMic();
    }
    public void VolumeBtn()
    {
        _isStop = !_isStop;
        if(_isStop )
        {
            MuteAllPlayers();
            volumImage.color = Color.black;
        }
        else
        {
            UnmuteAllPlayers();
            volumImage.color = Color.red;
        }
    }
    private void UpdateMic()
    {
        if (_isMute)
        {
            DisableVoice();
            micImage.color = Color.black; // Đặt màu đỏ khi mic bị tắt
        }
        else
        {
            EnableVoice();
            micImage.color = Color.red; // Đặt màu đen khi mic được bật
        }
    }

    public void DisableVoice()
    {
        if (NetworkManager.Instance.RecorderScr != null)
        {
            NetworkManager.Instance.RecorderScr.TransmitEnabled = false;
        }
        else
        {
            Debug.LogWarning("DisableVoice: Recorder là null - Không thể tắt mic vì không tìm thấy Recorder");
        }
    }

    public void EnableVoice()
    {
        if (NetworkManager.Instance.RecorderScr != null)
        {
            NetworkManager.Instance.RecorderScr.TransmitEnabled = true;
        }
    }
}
