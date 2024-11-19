using Fusion;
using UnityEngine;
using multiplayerMode;
namespace multiplayerMode
{
   
    public class WeaponHandler : NetworkBehaviour
    {
        public GameObject hitVFX;
        [SerializeField]
        private int damage = 10;
        [SerializeField]
        private Transform _shootPoint;

        [SerializeField]
        private Transform _leftShootPoint;

        [SerializeField]
        private LayerMask _hitLayer;

        [SerializeField]
        private HitOptions _hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;

        void SetupAttribute()
        {
            if (Object.HasStateAuthority)
            {
                if (PlayFabManager.Instance.UserData.PlayerAttributes.TryGetValue(UserEquipmentData.Instance.CurrentModelId, out AttributeData heroAttributes))
                {
                    HeroAttributeData heroAttributeData = PlayFabManager.Instance.UserData.GetHeroAttributeData();
                    if (heroAttributeData != null)
                    {
                        damage = heroAttributeData.Damage;
                    }
                }
            }
            
        }
        public override void Spawned()
        {
            SetupAttribute();

        }
        public void PistolRightFire()
        {
            if (Runner.LagCompensation.Raycast(_shootPoint.position,
                        _shootPoint.forward,
                        Mathf.Infinity,
                        Object.InputAuthority,
                        out LagCompensatedHit hit,
                        _hitLayer,
                        _hitOptions))
            {
                    if (hit.GameObject.TryGetComponent<PlayerController>(out var hitPlayerController))
                {
                        if(NetworkManager.Instance.IsTeamMode)
                        {
                            PlayerTeamSetup ePlayerTeamSetup = hitPlayerController.gameObject.GetComponent<PlayerTeamSetup>();

                            PlayerTeamSetup myPlayerTeamSetup = GetComponent<PlayerTeamSetup>();
                            if (myPlayerTeamSetup.teamID != null)
                            {
                                if (myPlayerTeamSetup.teamID == ePlayerTeamSetup.teamID)
                                {
                                    return;
                                }
                            }
                        }
                    
                   

                        string playerName = GameManager.Instance.PlayerData.playerName;
                        // Sử dụng RPC để thông báo cho client của người bị bắn gọi hàm TakeDamage và truyền vị trí va chạm
                        hitPlayerController.TakeDamage_RPC(damage, hit.Point, hit.Normal, playerName);
                        /*GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                        Destroy(vfx, 1f);*/
                }
                if (hit.GameObject.TryGetComponent<IDamageable>(out var hitBossNetworked))
                {
                        Debug.Log("treantMinion_ nguoi bi ban: "+hit.GameObject.name);
                        // Sử dụng RPC để thông báo cho client của người bị bắn gọi hàm TakeDamage và truyền vị trí va chạm

                        string playerName = GameManager.Instance.PlayerData.playerName;

                        hitBossNetworked.TakeDamage(damage, hit.Point, hit.Normal, playerName,NetworkManager.Instance.TeamID);
                   /* GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                    Destroy(vfx, 1f);*/
                }
                if (hit.GameObject.TryGetComponent<IEnvironmentInteractable>(out var hitIEnvironmentInteractable))
                {
                    Debug.Log("IEnvironmentInteractable:sung luc");
                    hitIEnvironmentInteractable.OnHitByWeapon();

                    /*GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                    Destroy(vfx, 1f);*/
                }
                if (hit.GameObject.TryGetComponent<IstatisEvm>(out var hitIEnvironment))
                {
                    hitIEnvironment.StaticEVM(hit.Point, hit.Normal);
                    return;
                    /*GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                    Destroy(vfx, 1f);*/
                }
                SethitVFX_RPC(hit.Point);
            }
        }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void SethitVFX_RPC(Vector3 posititon)
        {
            GameObject vfx = Instantiate(hitVFX, posititon, Quaternion.identity);
            Destroy(vfx, 1f);          
        }
        public void PistolLeftFire()
        {
            if (Runner.LagCompensation.Raycast(_leftShootPoint.position,
                        _leftShootPoint.forward,
                        Mathf.Infinity,
                        Object.InputAuthority,
                        out LagCompensatedHit hit,
                        _hitLayer,
                        _hitOptions))
            {

                if (hit.GameObject.TryGetComponent<PlayerController>(out var hitPlayerController))
                {
                        if (NetworkManager.Instance.IsTeamMode)
                        {
                            PlayerTeamSetup ePlayerTeamSetup = hitPlayerController.gameObject.GetComponent<PlayerTeamSetup>();

                            PlayerTeamSetup myPlayerTeamSetup = GetComponent<PlayerTeamSetup>();
                            if (myPlayerTeamSetup.teamID != null)
                            {
                                if (myPlayerTeamSetup.teamID == ePlayerTeamSetup.teamID)
                                {
                                    return;
                                }
                            }
                        }

                        string playerName = GameManager.Instance.PlayerData.playerName;
                        // Sử dụng RPC để thông báo cho client của người bị bắn gọi hàm TakeDamage và truyền vị trí va chạm
                        hitPlayerController.TakeDamage_RPC(damage, hit.Point, hit.Normal, playerName);
                   /* GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                    Destroy(vfx, 1f);*/
                }
                if (hit.GameObject.TryGetComponent<IDamageable>(out var hitBossNetworked))
                {

                    string playerName = GameManager.Instance.PlayerData.playerName;

                    hitBossNetworked.TakeDamage(damage, hit.Point, hit.Normal, playerName, NetworkManager.Instance.TeamID);
                    /*GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                    Destroy(vfx, 1f);*/
                }
                if (hit.GameObject.TryGetComponent<IEnvironmentInteractable>(out var hitIEnvironmentInteractable))
                {
                    Debug.Log("IEnvironmentInteractable:sung luc");

                    hitIEnvironmentInteractable.OnHitByWeapon();
                    /*GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                    Destroy(vfx, 1f);*/
                }
                if (hit.GameObject.TryGetComponent<IstatisEvm>(out var hitIEnvironment))
                {
                    hitIEnvironment.StaticEVM(hit.Point, hit.Normal);
                    return;
                    /*GameObject vfx = Instantiate(hitVFX, hit.Point, Quaternion.identity);
                    Destroy(vfx, 1f);*/
                }

                SethitVFX_RPC(hit.Point);
            }
        }

    }
}

