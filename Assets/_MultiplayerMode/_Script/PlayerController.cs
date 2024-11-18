using Fusion;
using OculusSampleFramework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using multiplayerMode;
using System.Collections;
using UnityEngine.SceneManagement;
namespace multiplayerMode
{
    public class PlayerController : NetworkBehaviour
    {
        [Networked] public string BossName { get; set; }

        [Networked]
        public int RubyNum {  get; set; }
        [SerializeField]
        private Transform _playerModels;
        [Networked] public string TeamID { get; set; }
        [SerializeField]
        private WeaponManager _weaponManager;
        //Equipment
        [Header("SetUp Equipment")]
        [SerializeField] private Transform _modelParentObject;
        [Networked]
        private string _playerModelName { get; set; }

        //UI
        [SerializeField] private GameObject rewardCanvas;
        [SerializeField] private TMP_Text quitTimeCountText;
        [SerializeField] private RewardPanel _rewardPanel;

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
        public override void Spawned()
        {
            _playerRef = Object.InputAuthority;
            if (!PlayerNames.ContainsKey(_playerRef))
            {
                // Nếu chưa tồn tại, thêm _playerRef và tên người chơi vào từ điển
                PlayerNames.Add(_playerRef, GameManager.Instance.PlayerData.playerName);
            }
            if (PlayerNames.ContainsKey(_playerRef))
            {
                string playerName = PlayerNames[_playerRef];

                _playerNameText.text = playerName;
            }
            Debug.Log("dev14_1xxxxx" + playerName);
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
            if (Object.HasStateAuthority)
            {

                _PlayerDict.Add(_playerRef, NetworkManager.Instance.PlayerSpawnerScript._networkPlayerObject);

                //----------------------------------------------------------------
                hardwareRig = FindObjectOfType<HardwareRig>();
                
                //cap nhat leaderboard
                GameManager.Instance.PlayerData.playerRef = _playerRef;
                _playerData = GameManager.Instance.PlayerData;
                playerName = _playerData.playerName;
                if (NetworkManager.Instance.IsTeamMode)
                {
                    TeamID = NetworkManager.Instance.TeamID;
                    RPC_SendTeamID(NetworkManager.Instance.TeamID);
                }
                UpdateLeaderboard_RPC(_playerData.playerName);
                //-------------------------------------------------------
            }
            if (Object.HasInputAuthority)
            {
                _playerModelName = UserEquipmentData.Instance.CurrentModelId;
                SetUpPlayerEquipment_RPC(_playerModelName);

                if (NetworkManager.Instance.BossName != null)
                {
                    BossName = NetworkManager.Instance.BossName;
                    RubyNum = 0;
                }
                else
                {
                    BossName = null;
                }
            }
            else
            {
                SetUpPlayerEquipmentOther(_playerModelName);
            }
            SetupAttribute();
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = _fireSound;
        }
        private Transform[] spawnPos;
        private void Start()
        {
            if(!NetworkManager.Instance.IsTeamMode)
            {
                GameObject obj = GameObject.FindWithTag("Intermediary");
                if (obj != null)
                {
                    Intermediary intermediary = obj.GetComponent<Intermediary>();
                    spawnPos = intermediary.spawnPostions;
                }
                else
                {
                    Debug.Log("No object found with the specified tag.");
                }
            }
            if(!NetworkManager.Instance.IsTeamMode)
            {
                if (NetworkManager.Instance.TeamID != null)
                {
                    Invoke("SetupPlayerNameColor", 1f);

                }
            }
            
           // Invoke("OnQuitButtonClick", 5f);
        }
        void SetupAttribute()
        {
            if(Object.HasStateAuthority)
            {
                HeroAttributeData heroAttributeData = PlayFabManager.Instance.UserData.GetHeroAttributeData();
                if (heroAttributeData != null)
                {
                    _maxHp = heroAttributeData.HP;
                    _currentHp = _maxHp;
                }
            }
           
        }
        void SetupPlayerNameColor()
        {
            PlayerTeamSetup playerTeamSetup = GetComponent<PlayerTeamSetup>();
            playerTeamSetup.SetPlayerNameColor(TeamID);
        }
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Debug.Log($"dev10_2OnPlayerLeft {_PlayerDict.Count}");
            
            if (Object.HasStateAuthority)
            {
                // Xóa người chơi khỏi danh sách PlayerNames
                if (PlayerNames.ContainsKey(_playerRef))
                {
                    PlayerNames.Remove(_playerRef);
                    if (_PlayerDict.ContainsKey(_playerRef))
                    {
                        NetworkObject networkObject = _PlayerDict[_playerRef];
                        
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
      

        // Hàm callback được gọi khi CurrentWeapon thay đổi
        private static void OnWeaponChanged(Changed<PlayerController> changed)
        {
            changed.Behaviour.OnWeaponChanged();
        }

        private void OnWeaponChanged()
        {
            if(Object.HasStateAuthority)
            {
                // Cập nhật vũ khí trong WeaponManager
                //_weaponManager.SwitchWeapon(CurrentWeapon);
            }
            
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
                _currentHp += newHp;              
                if (_currentHp > _maxHp)
                {
                    _currentHp = _maxHp;
                }
            }
        }
        private void UpdateHpUI(int hp)
        {
            if (Object.HasInputAuthority)
            {
                hardwareRig.UpdateHP(hp,_maxHp);
            }
        }
        
       
        private void SetUpPlayerEquipmentOther(string playerModelName)
        {
            Transform modelObject = _playerModels.Find(playerModelName);


            if (modelObject != null)
            {
                foreach (Transform child in _playerModels)
                {
                    if (child.name != playerModelName)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Log("dev20_ Không tìm thấy đối tượng con 'mage'.");
            }
        }
       
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void SetUpPlayerEquipment_RPC(string playerModelName)
        {
            Debug.Log($"checkState_ playerName: {playerName} playerModelName:{playerModelName}");
            Transform modelObject = _playerModels.Find(playerModelName);

            
            if (modelObject != null)
            {
                foreach (Transform child in _playerModels)
                {
                    if (child.name != playerModelName)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Log("dev20_ Không tìm thấy đối tượng con 'mage'.");
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SendTeamID(string teamID)
        {
            // Gán TeamID từ tham số truyền vào
            TeamID = teamID;
            
            
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
       /* private void Start()
        {
            Invoke("OnQuitButtonClick", 5f);
        }*/
        public void OnQuitButtonClick()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

           
            if (Object.InputAuthority.IsValid && currentSceneName == "MainGame")
            {
                
                int playerScore = hardwareRig.GetPlayerScore(playerName);
                // Gọi hàm AddGoldCoin và xử lý số coin thực tế đã thêm qua callback
                if(playerScore != 0)
                {
                    PlayFabManager.Instance.CurrencyManager.AddGoldCoin(playerScore, (int coinsAdded) =>
                    {
                        rewardCanvas.SetActive(true);
                       
                        _rewardPanel.SpawnRewardItem(ItemRewardType.Coin, coinsAdded);
                        //_rankCointText.text = "+"+coinsAdded.ToString();  // Hiển thị số coin thực tế nhận được
                    });
                }
                else
                {
                    rewardCanvas.SetActive(true);
                    _rewardPanel.SpawnRewardItem(ItemRewardType.Coin, 0);

                }


                StartCoroutine(QuitCountdown());
            }
            else
            {
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
        //Bosss
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_TakeDamage(int damage)
        {
            TakeDamage_Boss(damage);
        }
       public void KillBossReward()
        {
            if(Object.HasInputAuthority)
            {
                RewardAndExitBoss();

            }
        }
        void RewardAndExitBoss()
        {
            PlayFabManager.Instance.CurrencyManager.AddGoldCoin(15, (int coinsAdded) =>
            {
                rewardCanvas.SetActive(true);
                _rewardPanel.SpawnRewardItem(ItemRewardType.Coin, coinsAdded);
                if (RubyNum > 0)
                {
                    rewardCanvas.SetActive(true);
                    if (NetworkManager.Instance.BossName == "Boss1")
                    {
                        _rewardPanel.SpawnRewardItem(ItemRewardType.Amethyst, RubyNum);
                        Item newItem = new Item("Amethyst", ItemType.Ruby, RubyNum);
                        UserEquipmentData.Instance.AddItem(newItem);
                    }
                    else if (NetworkManager.Instance.BossName == "Boss2")
                    {
                        _rewardPanel.SpawnRewardItem(ItemRewardType.Emerald, RubyNum);
                        Item newItem = new Item("Emerald", ItemType.Ruby, RubyNum);
                        UserEquipmentData.Instance.AddItem(newItem);
                    }
                    else if (NetworkManager.Instance.BossName == "Boss3")
                    {
                        _rewardPanel.SpawnRewardItem(ItemRewardType.Sapphire, RubyNum);
                        Item newItem = new Item("Sapphire", ItemType.Ruby, RubyNum);
                        UserEquipmentData.Instance.AddItem(newItem);
                    }

                }
            });


            StartCoroutine(QuitCountdown());
        }
        public void ExitBoss()
        {
            
            if(RubyNum > 0)
            {
                rewardCanvas.SetActive(true);
                if(NetworkManager.Instance.BossName == "Boss1")
                {
                    _rewardPanel.SpawnRewardItem(ItemRewardType.Amethyst, RubyNum);
                    Item newItem = new Item("Amethyst", ItemType.Ruby, RubyNum);
                    UserEquipmentData.Instance.AddItem(newItem);
                }
                else if(NetworkManager.Instance.BossName == "Boss2")
                {
                    _rewardPanel.SpawnRewardItem(ItemRewardType.Emerald, RubyNum);
                    Item newItem = new Item("Emerald", ItemType.Ruby, RubyNum);
                    UserEquipmentData.Instance.AddItem(newItem);
                }
                else if(NetworkManager.Instance.BossName == "Boss3")
                {
                    _rewardPanel.SpawnRewardItem(ItemRewardType.Sapphire, RubyNum);
                    Item newItem = new Item("Sapphire", ItemType.Ruby, RubyNum);
                    UserEquipmentData.Instance.AddItem(newItem);
                }
               
            }
            StartCoroutine(QuitCountdown());

        }
        /* [Rpc(RpcSources.All, RpcTargets.All)]
         public void KillBossReward_RPC(string shooterName)
         {

             if (shooterName == playerName)
             {
                 Debug.Log("bossreward_ 6");
                 TeamKillBossReward_RPC(TeamID);
             }
         }*/
        /*[Rpc(RpcSources.All, RpcTargets.All)]
        public void TeamKillBossReward_RPC(string teamID)
        {
            if(TeamID == teamID)
            {
                Debug.Log("bossreward_ team");

            }
        }*/
        public void TakeDamage_Boss(int damage)
        {
           /* Debug.Log("checkquyenhan_HasInputAuthority: " + Object.HasInputAuthority +" "+playerName);
            Debug.Log("checkquyenhan_HasStateAuthority: " + Object.HasStateAuthority + " " + playerName);
*/

            if (Object.HasStateAuthority)
            {
                _currentHp -= damage;

                if (_currentHp <= 0)
                {
                    _currentHp = 0;
                    Dead();
                    Invoke("SetInitHp", 1f);
                }
            }
        }
        public void TakeDamage_Boss_Name(int damage)
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log("Bossfixbug_ 3_" + playerName);
                _currentHp -= damage;

                Debug.Log("Bossfixbug_ 4_" + playerName + _currentHp);

                if (_currentHp <= 0)
                {
                    _currentHp = _maxHp;
                    /*Dead();
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
            if(Object.HasStateAuthority)
            {
                _currentHp = _maxHp; // Đặt lại HP khi player chết

            }
        }
        
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            if (Object.HasStateAuthority)
            {
                if (transform.root.position.y < -20f)
                {
                    int randomIndex = Random.Range(0, spawnPos.Length);
                    Invoke("SetInitHp", 1f);
                    hardwareRig.Death(spawnPos[randomIndex].position);
                }
            }
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
            if(Object.HasStateAuthority)
            {
                if (NetworkManager.Instance.IsTeamMode)
                {
                    Vector3 pos = new Vector3(0, 5, 0);

                    hardwareRig.Death(pos);

                    return;
                }               
                int randomIndex = Random.Range(0, spawnPos.Length);
                hardwareRig.Death(spawnPos[randomIndex].position);
            }
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
        // Hàm nhận tác động vật lý từ bên ngoài
        public void ReceiveImpact(Vector3 direction, float force)
        {
            if(Object.HasStateAuthority)
            {
                hardwareRig.ReceiveImpact(direction, force);

            }
        }

    }
}