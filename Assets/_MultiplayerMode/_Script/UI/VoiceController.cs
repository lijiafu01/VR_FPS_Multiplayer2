using UnityEngine;
using Photon.Voice.Unity;
using multiplayerMode;
using UnityEngine.UI;
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
        PlayFabManager.Instance.SaveSpeakerOnOff(_isStop);

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
        PlayFabManager.Instance.SaveMicOnOff(_isMute);

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
    //==================================================Game sound ==================================
    [SerializeField] private Slider volumeSlider; // Tham chiếu đến Slider UI
    private void Start()
    {
        _isMute = false;
        _isStop = false;
        AudioListener.volume = 1.0f;    
        volumeSlider.value = AudioListener.volume;

        PlayFabManager.Instance.LoadMicOnOff((bool micOnOff) =>
        {
            _isMute = !micOnOff;
            MicBtn();
        });

        PlayFabManager.Instance.LoadSpeakerOnOff((bool speakerOnOff) =>
        {
            _isStop = !speakerOnOff;
            VolumeBtn();
        });
        // Kiểm tra nếu volumeSlider chưa được gán
        if (volumeSlider == null)
        {
            Debug.LogError("MasterVolumeController: Slider chưa được gán.");
            return;
        }
        PlayFabManager.Instance.LoadVolume((float volume) =>
        {
            AudioListener.volume = volume;
            // Đặt giá trị mặc định cho slider từ âm lượng hiện tại       
            volumeSlider.value = AudioListener.volume;
        });      
       
        // Thêm listener để thay đổi âm lượng khi kéo slider
        volumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }
    // Hàm điều chỉnh âm lượng toàn bộ game
    private void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayFabManager.Instance.SaveVolume(volume);
        //Debug.Log("Âm lượng toàn bộ game hiện tại: " + volume);
    }
    // Hàm để gỡ bỏ listener khi script hoặc object bị hủy
    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        }
    }
}
