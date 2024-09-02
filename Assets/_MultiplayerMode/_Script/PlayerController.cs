using Fusion;
using OculusSampleFramework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public GameObject menuPanel;
    [SerializeField] private TMP_Text _playerNameText;
    //[Networked] public NetworkString<_16> NickName { get; set; }
    [SerializeField]
    private int _maxHp = 100;
    [Networked] // Sử dụng Networked để đồng bộ HP giữa các client
    public int _currentHp { get; set; }
    [SerializeField]
    private WeaponHandler _weaponHandler;
    private NetworkButtons _previousButton { get; set; }

    private HardwareRig hardwareRig;
    [SerializeField]
    private AudioClip _fireSound;
    private AudioSource _audioSource;
    // Thêm biến cho ParticleSystem
    [SerializeField]
    private ParticleSystem _muzzleFlash;

    [SerializeField]
    private ParticleSystem _leftMuzzleFlash;
    [Networked]
    public string playerName { get; set; }
    // Thêm biến cho hiệu ứng máu
    [SerializeField]
    private ParticleSystem _bloodEffect;

    private PlayerData _playerData;
    private PlayerRef _playerRef;
    [Networked, Capacity(30)]
    public NetworkDictionary<PlayerRef, string> PlayerNames => default;
    [Networked, Capacity(20)]
    public NetworkDictionary<PlayerRef, NetworkObject> _PlayerDict => default;
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Debug.Log($"dev10_2OnPlayerLeft {_PlayerDict.Count}");
        //Debug.Log($"dev12_1player {_playerRef.PlayerId} da bi xoa");
        // Kiểm tra xem người chơi này có phải là người đang giữ quyền trạng thái (State Authority) không
        if (Object.HasStateAuthority)
        {
            // Xóa người chơi khỏi danh sách PlayerNames
            if (PlayerNames.ContainsKey(_playerRef))
            {
                PlayerNames.Remove(_playerRef);
                if (_PlayerDict.ContainsKey(_playerRef))
                {
                    NetworkObject networkObject = _PlayerDict[_playerRef];
                    // Sử dụng runner để despawn đối tượng
                    //runner.Despawn(networkObject);
                    // Xóa đối tượng khỏi dictionary
                    // Gửi RPC để thông báo cho tất cả client xóa đối tượng này
                    //RemovePlayerOnClients_RPC(_playerRef); 
                    //_PlayerDict.Remove(_playerRef);
                }
            }
        }
        if (Object.HasStateAuthority) return;
        Debug.Log("dev3_co cap nhat lai BXH");
        HardwareRig hardwareRig = FindObjectOfType<HardwareRig>();
        if (hardwareRig != null)
        {
            // Gọi hàm cập nhật BXH với thông tin của player
            hardwareRig.RemovePlayerFromLeaderboard(playerName);
            Debug.Log($"dev3_Player {playerName} has been removed from leaderboard.");
        }
    }
    /*[Rpc(RpcSources.All, RpcTargets.All)]
    private void RemovePlayerOnClients_RPC(PlayerRef playerRef)
    {
        Debug.Log($"dev8_2Player {playerRef.PlayerId} removed from client.");
        // Chỉ xử lý trên các client khác (ngoại trừ người gửi)
        if (playerRef != Runner.LocalPlayer)
        {
            Debug.Log($"dev8_2Player {playerRef.PlayerId} removed from client.");
            if (_PlayerDict.ContainsKey(playerRef))
            {
                NetworkObject networkObject = _PlayerDict[playerRef];

                // Sử dụng runner để despawn đối tượng
                NetworkManager.Instance.Runner.Despawn(networkObject);

                // Xóa đối tượng khỏi dictionary
                _PlayerDict.Remove(playerRef);
                Debug.Log($"dev8_3Player {playerRef.PlayerId} removed from client.");
            }
        }
    }*/
        /*[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RemovePlayerFromLeaderboard_RPC(string playerName)
        {
            // Tìm HardwareRig và cập nhật BXH trên mỗi client
            HardwareRig hardwareRig = FindObjectOfType<HardwareRig>();
            if (hardwareRig != null)
            {
                // Gọi hàm cập nhật BXH với thông tin của player
                hardwareRig.RemovePlayerFromLeaderboard(playerName);
            }
            else
            {
                Debug.LogError("HardwareRig not found!");
            }
        }*/
    public override void Spawned()
    {
        _playerRef = Object.InputAuthority;
        
        /* if (HasInputAuthority)
         {
             PlayerNames.Add(_playerRef, GameManager.Instance.PlayerData.playerName);
            // _playerNameText.text = GameManager.Instance.PlayerData.playerName;
         }*/
        // Kiểm tra xem PlayerRef đã tồn tại trong từ điển PlayerNames hay chưa
        if (!PlayerNames.ContainsKey(_playerRef))
        {
            // Nếu chưa tồn tại, thêm PlayerRef và tên người chơi vào từ điển
            PlayerNames.Add(_playerRef, GameManager.Instance.PlayerData.playerName);
            foreach (var kvp in PlayerNames)
            {
                Debug.Log($"dev_Khóa: {kvp.Key}, Tên người chơi: {kvp.Value}");
            }
        }
        if (PlayerNames.ContainsKey(_playerRef))
        {
 
            string playerName = PlayerNames[_playerRef];

            // Gán tên người chơi vào Text UI
            _playerNameText.text = playerName;
        }
        //Rpc_SetNickname(GameManager.Instance.PlayerData.playerName);
        if (Runner.LocalPlayer == GetHostPlayerRef())
        {
            hardwareRig = FindObjectOfType<HardwareRig>();
            Dictionary<string, int> playerScores = hardwareRig._playerScores;
            // Gọi RPC để gửi thông tin người chơi tới tất cả người chơi
            foreach (var entry in playerScores)
            {
                SendPlayerDataToAll_RPC(entry.Key, entry.Value);
            }
        }
        if (Object.HasStateAuthority)
        {
            
            _PlayerDict.Add(_playerRef, NetworkManager.Instance.PlayerSpawnerScript._networkPlayerObject);
            
            //----------------------------------------------------------------
            hardwareRig = FindObjectOfType<HardwareRig>();
            _currentHp = _maxHp; 
            //cap nhat leaderboard
            GameManager.Instance.PlayerData.playerRef = _playerRef;
            _playerData = GameManager.Instance.PlayerData;
            playerName = _playerData.playerName;
            UpdateLeaderboard_RPC(_playerData.playerName);
            //-------------------------------------------------------
        }              
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = _fireSound;
    }
    private void Update()
    {
        if (Object.HasStateAuthority)
        {
            if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
            }
        }
        
    }
    
    public void OnQuitButtonClick()
    {
        LocalManager.Instance.LoadStartGameScene();
    }
    public PlayerRef GetHostPlayerRef()
    {
        PlayerRef host = PlayerRef.None;

        foreach (var player in Runner.ActivePlayers)
        {
            if (host == PlayerRef.None || player < host)
            {
                host = player;
            }
        }

        return host;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void SendPlayerDataToAll_RPC(string playerName, int playerScore)
    {

        // Gọi hàm trong HardwareRig để cập nhật bảng xếp hạng
        HardwareRig hardwareRig = FindObjectOfType<HardwareRig>();
        if (hardwareRig != null)
        {
            hardwareRig.AddOrUpdatePlayerOnLeaderboardWithScore(playerName, playerScore);
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void UpdateLeaderboard_RPC(string playerName)
    {
        // Tìm HardwareRig và cập nhật BXH trên mỗi client
        HardwareRig hardwareRig = FindObjectOfType<HardwareRig>();
        if (hardwareRig != null)
        {
            // Gọi hàm cập nhật BXH với thông tin của player
            hardwareRig.AddOrUpdatePlayerOnLeaderboard(playerName);
        }
        else
        {
            Debug.LogError("HardwareRig not found!");
        }
    }
    public void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal,string shooterName)
    {
        if (Object.HasStateAuthority)
        {
            _currentHp -= damage;

            // Hiển thị hiệu ứng máu khi bị bắn
            PlayBloodEffect_RPC(hitPosition, hitNormal);

            if (_currentHp <= 0)
            {
                Dead();
                UpdateLeaderboard_RPC(shooterName);
                _currentHp = _maxHp; // Đặt lại HP khi player chết
            }
        }
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (GetInput<RigState>(out var input))
        {
            var buttonPressed = input.Button.GetPressed(_previousButton);
            _previousButton = input.Button;
            if (buttonPressed.IsSet(InputButton.Fire))
            {
                FirePistolRight();
            }
            if (buttonPressed.IsSet(InputButton.Fire2))
            {
                FirePistolLeft();
            }
        }
    }
    private void FirePistolRight()
    {
        _weaponHandler.PistolRightFire();

        // Gọi hàm để chơi âm thanh và hiển thị hiệu ứng tóe lửa
        PistolRightFireEffects_RPC();
    }
    private void FirePistolLeft()
    {
        _weaponHandler.PistolLeftFire();

        // Gọi hàm để chơi âm thanh và hiển thị hiệu ứng tóe lửa
        PistolLeftFireEffects_RPC();
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void PistolRightFireEffects_RPC()
    {
        if (_audioSource != null && _fireSound != null)
        {
            _audioSource.Play();
        }
        _muzzleFlash.Play();
        Invoke(nameof(StopPistolRightMuzzleFlash), 0.1f); // Dừng hiệu ứng sau 1 giây
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void PistolLeftFireEffects_RPC()
    {
        if (_audioSource != null && _fireSound != null)
        {
            _audioSource.Play();
        }

        _leftMuzzleFlash.Play();
        Invoke(nameof(StopPistolLeftMuzzleFlash), 0.1f); // Dừng hiệu ứng sau 1 giây
    }
    private void StopPistolRightMuzzleFlash()
    {
        _muzzleFlash.Stop();
    }
    private void StopPistolLeftMuzzleFlash()
    {
        _leftMuzzleFlash.Stop();
    }
    private void Dead()
    {
        Vector3 pos1 = new Vector3(0, 5, 0);
        Vector3 pos2 = new Vector3(29, 5, 50);
        Vector3 pos3 = new Vector3(58, 5, -10);

        Vector3[] positions = new Vector3[] { pos1, pos2, pos3 };
        int randomIndex = Random.Range(0, positions.Length);
        hardwareRig.gameObject.transform.position = positions[randomIndex];

    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TakeDamage_RPC(int damage, Vector3 hitPosition, Vector3 hitNormal,string shooterName)
    {
        TakeDamage(damage, hitPosition, hitNormal, shooterName);
    }   
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void PlayBloodEffect_RPC(Vector3 hitPosition, Vector3 hitNormal)
    {

        if (_bloodEffect != null)
        {
            // Đặt vị trí và hướng của ParticleSystem dựa trên vị trí va chạm và pháp tuyến
            _bloodEffect.transform.position = hitPosition;
            _bloodEffect.transform.rotation = Quaternion.LookRotation(hitNormal);

            _bloodEffect.Play();
            Invoke(nameof(StopBloodEffect), 1f); // Dừng hiệu ứng sau 1 giây
        }
        else
        {
            Debug.Log("dev_blood_effect_is_null");
        }
    }
    private void StopBloodEffect()
    {
        if (_bloodEffect != null)
        {
            _bloodEffect.Stop();
        }
    }

}
