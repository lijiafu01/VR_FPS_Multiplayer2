using UnityEngine;
using Photon.Voice.Unity;
using multiplayerMode;

public class VoiceController : MonoBehaviour
{
    private Recorder recorder = null;
    private bool _isMute = false;
    private void Start()
    {
        Invoke("Init", 1.3f);
        Invoke("DisableVoice", 10f);
    }
    void Init()
    {
        recorder = NetworkManager.Instance.RecorderScr;

    }

    public void MicBtn()
    {
        _isMute = !_isMute;
        UpdateMic();
    }
    private void UpdateMic()
    {
        if (_isMute)
        {
            DisableVoice();
        }
        else
        {
            EnableVoice();
        }
    }

    public void DisableVoice()
    {
        Debug.Log("DisableVoice: Bắt đầu phương thức");

        if (recorder != null)
        {
            Debug.Log("DisableVoice: Recorder không null, tiếp tục tắt mic");

            recorder.RecordingEnabled = false;
            Debug.Log("DisableVoice: RecordingEnabled đã được đặt thành false");

            recorder.TransmitEnabled = false;
            Debug.Log("DisableVoice: TransmitEnabled đã được đặt thành false");

            Debug.Log("DisableVoice: Mic đã được tắt thành công");

        }
        else
        {
            Debug.LogWarning("DisableVoice: Recorder là null - Không thể tắt mic vì không tìm thấy Recorder");
        }
    }


    public void EnableVoice()
    {
        if (recorder == null) return;
        recorder.RecordingEnabled = true;
        recorder.TransmitEnabled = true;
    }

}
