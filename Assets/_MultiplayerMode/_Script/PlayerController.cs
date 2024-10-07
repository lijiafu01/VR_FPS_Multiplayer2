using Fusion;
using OculusSampleFramework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using multiplayerMode;
using System.Collections;
namespace multiplayerMode
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField]
        private WeaponManager _weaponManager;
        //Equipment
        [Header("SetUp Equipment")]
        [SerializeField] private Transform _modelParentObject;
       /* [Networked]
        private string _playerModelName { get; set; }*/

        //UI
        [SerializeField] private GameObject rewardCanvas;
        [SerializeField] private TMP_Text quitTimeCountText;
        [SerializeField] private TMP_Text _rankCointText;

        // Biến mạng để lưu trữ vũ khí hiện tại
        [Networked(OnChanged = nameof(OnWeaponChanged))]
        public Weapon CurrentWeapon { get; set; }

        /* private TextMeshProUGUI hpText;
         [SerializeField]
         private GameObject hpCanvas;*/
        public GameObject menuPanel;
        [SerializeField] private TMP_Text _playerNameText;
        //[Networked] public NetworkString<_16> NickName { get; set; }
        [SerializeField]
        private int _maxHp = 100;
        [Networked(OnChanged = nameof(OnHpChanged))]
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
        public void SwitchWeapon(Weapon weapon)
        {
            Debug.Log($"dev1x_{playerName} switch weapon to {weapon}");

            // Gọi RPC để thông báo cho tất cả các máy khách
            RPC_SwitchWeapon(weapon);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_SwitchWeapon(Weapon weapon)
        {
            // Cập nhật vũ khí hiện tại
            CurrentWeapon = weapon;

            // Chuyển đổi vũ khí trong WeaponManager
            _weaponManager.SwitchWeapon(weapon);
        }

        // Hàm callback được gọi khi CurrentWeapon thay đổi
        private static void OnWeaponChanged(Changed<PlayerController> changed)
        {
            changed.Behaviour.OnWeaponChanged();
        }

        private void OnWeaponChanged()
        {
            // Cập nhật vũ khí trong WeaponManager
            _weaponManager.SwitchWeapon(CurrentWeapon);
        }
        private static void OnHpChanged(Changed<PlayerController> changed)
        {
            // Lấy giá trị mới của _currentHp
            int newHp = changed.Behaviour._currentHp;

            Debug.Log($"HP has changed to {newHp}");

            // Gọi hàm để cập nhật UI hoặc xử lý các logic khác
            changed.Behaviour.UpdateHpUI(newHp);
        }
        public void IncreaseHealth(int newHp)
        {
            if (Object.HasStateAuthority)
            {
                if (_currentHp >= _maxHp) return;
                _currentHp += newHp;
                if (_currentHp < _maxHp)
                {
                    _currentHp = _maxHp;
                }
            }
        }
        private void UpdateHpUI(int hp)
        {
            hardwareRig.UpdateHP(hp);
        }
        public override void Spawned()
        {
            _playerRef = Object.InputAuthority;


            if (!PlayerNames.ContainsKey(_playerRef))
            {
                // Nếu chưa tồn tại, thêm PlayerRef và tên người chơi vào từ điển
                PlayerNames.Add(_playerRef, GameManager.Instance.PlayerData.playerName);

            }
            if (PlayerNames.ContainsKey(_playerRef))
            {

                string playerName = PlayerNames[_playerRef];

                _playerNameText.text = playerName;
            }
            Debug.Log("dev14_1xxxxx" + playerName);

            //Rpc_SetNickname(GameManager.Instance.PlayerData.playerName);
           /* if (Runner.LocalPlayer == GetHostPlayerRef())
            {
                Debug.Log("dev14_2xxxxx" + playerName);
                hardwareRig = FindObjectOfType<HardwareRig>();
                Dictionary<string, int> playerScores = hardwareRig._playerScores;
                // Gọi RPC để gửi thông tin người chơi tới tất cả người chơi
                foreach (var entry in playerScores)
                {
                    SendPlayerDataToAll_RPC(entry.Key, entry.Value);
                }
            }*/
            if (!Object.HasStateAuthority)
            {
                Debug.Log("dev14_2xxxxx" + playerName);
                hardwareRig = FindObjectOfType<HardwareRig>();
                Dictionary<string, int> playerScores = hardwareRig._playerScores;
                // Gọi RPC để gửi thông tin người chơi tới tất cả người chơi
                foreach (var entry in playerScores)
                {
                    SendPlayerDataToAll_RPC(entry.Key, entry.Value);
                }
            }
            /*if(Object.InputAuthority.IsValid)
            {
                _playerModelName = UserEquipmentData.Instance.CurrentModelId;
                Debug.Log("dev20_" + _playerModelName);

            }*/
            if (Object.HasStateAuthority)
            {

                WeaponManager weaponManager = GetComponentInParent<WeaponManager>();
                CurrentWeapon = weaponManager.CurrenWeapon;
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

            //setup player Equipment
           // Invoke("SetUpPlayerEquipment", 0.3f);
            //SetUpPlayerEquipment();


            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = _fireSound;
        }
        /*void SetUpPlayerEquipment()
        {

            // Tìm và tải prefab từ thư mục Resources/ModelPrefabs
            GameObject modelPrefab = Resources.Load<GameObject>($"ModelPrefabs/{_playerModelName}");

            // Kiểm tra xem có tìm thấy prefab không
            if (modelPrefab != null)
            {
                // Tạo một object mới từ prefab
                GameObject modelInstance = Instantiate(modelPrefab, _modelParentObject);

                // Đặt vị trí, quay và tỉ lệ của object mới theo _modelParentObject
                modelInstance.transform.localPosition = Vector3.zero;
                modelInstance.transform.localRotation = Quaternion.identity;
                modelInstance.transform.localScale = Vector3.one;

                // Nếu cần, bạn có thể gắn thêm các hành vi hoặc thiết lập khác cho modelInstance tại đây
            }
            else
            {
                // Nếu không tìm thấy prefab, in ra thông báo lỗi
                Debug.LogError($"Model prefab with name {_playerModelName} not found in Resources/ModelPrefabs!");
            }
        }*/
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
       /* private void Start()
        {
            Invoke("OnQuitButtonClick", 5f);
        }*/
        public void OnQuitButtonClick()
        {
            if (Object.InputAuthority.IsValid)
            {
                
                int playerScore = hardwareRig.GetPlayerScore(playerName);
                // Gọi hàm AddGoldCoin và xử lý số coin thực tế đã thêm qua callback
                if(playerScore != 0)
                {
                    PlayFabManager.Instance.CurrencyManager.AddGoldCoin(playerScore, (int coinsAdded) =>
                    {
                        rewardCanvas.SetActive(true);
                        _rankCointText.text = "+"+coinsAdded.ToString();  // Hiển thị số coin thực tế nhận được
                    });
                }
                else
                {
                    rewardCanvas.SetActive(true);
                    _rankCointText.text = "+0";
                }

                
                StartCoroutine(QuitCountdown());
            }
        }


        private IEnumerator QuitCountdown()
        {
            int count = 3;
            
            if (quitTimeCountText != null)
            {
                // Đếm ngược và hiển thị giá trị trên TMP_Text
                while (count > 0)
                {
                    quitTimeCountText.text = count.ToString();
                    yield return new WaitForSeconds(1); // Đợi 1 giây
                    count--;
                }

                // Sau khi đếm ngược kết thúc, thực hiện hành động như chuyển cảnh
                LocalManager.Instance.LoadStartGameScene();
            }
            else
            {
                Debug.LogError("Không tìm thấy đối tượng Text_QuitTimeCount trong rewardCanvas");
            }
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

        public void TakeDamage_Boss(int damage)
        {
            if (Object.HasStateAuthority)
            {
                _currentHp -= damage;


                if (_currentHp <= 0)
                {
                    _currentHp = _maxHp;
                 /*   Dead();
                    Invoke("SetInitHp", 1f);*/
                }
            }
        }
        public void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName)
        {
            if (Object.HasStateAuthority)
            {
                _currentHp -= damage;

                // Hiển thị hiệu ứng máu khi bị bắn
                PlayBloodEffect_RPC(hitPosition, hitNormal);

                if (_currentHp <= 0)
                {
                    _currentHp = _maxHp;
                    Dead();
                    UpdateLeaderboard_RPC(shooterName);
                    Invoke("SetInitHp", 1f);
                }
            }
        }
        private void SetInitHp()
        {
            _currentHp = _maxHp; // Đặt lại HP khi player chết
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
                    if(CurrentWeapon == Weapon.Pistol)
                    {
                        FirePistolRight();

                    }
                }
                if (buttonPressed.IsSet(InputButton.Fire2))
                {
                    if (CurrentWeapon == Weapon.Pistol)
                    {
                        FirePistolLeft();
                    }
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
        public void TakeDamage_RPC(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName)
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
}