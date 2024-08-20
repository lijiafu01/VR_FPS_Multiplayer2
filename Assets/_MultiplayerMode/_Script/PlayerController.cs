using Fusion;
using OculusSampleFramework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
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

    // Thêm biến cho hiệu ứng máu
    [SerializeField]
    private ParticleSystem _bloodEffect;
    bool isRight = true;

    private PlayerData _playerData;
    private PlayerRef _playerRef;
    [Networked, Capacity(30)]
    public NetworkDictionary<PlayerRef, string> PlayerNames => default;
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
            
            //----------------------------------------------------------------
            hardwareRig = FindObjectOfType<HardwareRig>();
            _currentHp = _maxHp; 
            //cap nhat leaderboard
            _playerData = GameManager.Instance.PlayerData;
            UpdateLeaderboard_RPC(_playerData.playerName);
            //-------------------------------------------------------
        }
        
        
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = _fireSound;

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
                isRight = true;
                FireWeapon(isRight);
            }
            if (buttonPressed.IsSet(InputButton.Fire2))
            {
                isRight = false;
                FireWeapon(isRight);
            }
        }
    }

    private void FireWeapon(bool isRight)
    {
        _weaponHandler.Fire(isRight);

        // Gọi hàm để chơi âm thanh và hiển thị hiệu ứng tóe lửa
        FireWeaponEffects_RPC();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void FireWeaponEffects_RPC()
    {
        if (_audioSource != null && _fireSound != null)
        {
            _audioSource.Play();
        }

        if (isRight)
        {
            _muzzleFlash.Play();
            Invoke(nameof(StopMuzzleFlash), 0.3f); // Dừng hiệu ứng sau 1 giây

        }
        else
        {
            
            _leftMuzzleFlash.Play();
            Invoke(nameof(StopMuzzleFlash), 0.3f); // Dừng hiệu ứng sau 1 giây
        }
    }

    private void StopMuzzleFlash()
    {
        if(isRight)
        {
            _muzzleFlash.Stop();
        }
        if(!isRight)
        {
            _leftMuzzleFlash.Stop();
        }
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
