using UnityEngine;
using Fusion;

namespace multiplayerMode
{
    
    public class Arrow : NetworkBehaviour
    {
        public GameObject hitVFX;
        [Networked] private NetworkBehaviourId hitObjectId { get; set; }
        private Transform hitTransform;
        private Vector3 localPosition;
        private Quaternion localRotation;

        private Rigidbody rb;  // Reference to the Rigidbody component
        [SerializeField] private float penetrationDepth = 0.5f; // Configurable penetration depth
        private Vector3 lastVelocity; // Biến để lưu vận tốc cuối cùng trước khi va chạm
        private CapsuleCollider bowCollider;

        [Networked] private TickTimer DestroyTimer { get; set; } // Thời gian hủy mũi tên (đồng bộ hóa)

        [SerializeField] private int damage = 10; // Số sát thương mũi tên gây ra
        [SerializeField] private LayerMask hitLayer; // Lớp đối tượng có thể nhận sát thương (người chơi)
        [SerializeField] private HitOptions hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
        void SetupAttribute()
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
        void Start()
        {
            SetupAttribute();
            rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
            bowCollider = GetComponent<CapsuleCollider>();

            if (bowCollider != null)
            {
                bowCollider.isTrigger = true;
            }
            else
            {
                Debug.LogError("bowCollider is not assigned!");
            }

            if (rb != null)
            {
                rb.isKinematic = false;  // Đảm bảo Rigidbody không ở trạng thái kinematic để nó chịu ảnh hưởng vật lý
            }
            else
            {
                Debug.LogError("Rigidbody (rb) is not assigned!");
            }

            // Thiết lập bộ đếm thời gian tự hủy (ví dụ 5 giây)
            if (Object.HasStateAuthority)
            {
                DestroyTimer = TickTimer.CreateFromSeconds(Runner, 5f);  // Tăng thời gian tự hủy để đảm bảo mũi tên tồn tại đủ lâu
            }
        }

        public override void FixedUpdateNetwork()
        {
           /* if (rb != null && !rb.isKinematic && rb.velocity != Vector3.zero)
            {
                lastVelocity = rb.velocity;  // Lưu lại vận tốc thực sự, không chỉ hướng

                // Thực hiện raycast để kiểm tra va chạm với người chơi
                RaycastForPlayerHit();
            }*/

            // Kiểm tra thời gian hủy mũi tên
            if (DestroyTimer.Expired(Runner))
            {
                Runner.Despawn(Object); // Hủy mũi tên sau khi hết thời gian
            }
        }

        private void RaycastForPlayerHit()
        {
            // Thực hiện raycast từ vị trí của mũi tên theo hướng di chuyển của nó
            if (Runner.LagCompensation.Raycast(transform.position,
                    lastVelocity.normalized,
                    Mathf.Infinity,
                    Object.InputAuthority,
                    out LagCompensatedHit hit,
                    hitLayer,
                    hitOptions))
            {
                // Xử lý va chạm với người chơi
                //ProcessPlayerHit(hit);
            }
        }
        //bool isShoot = false;
        /*private void ProcessPlayerHit(LagCompensatedHit hit)
        {
            Destroy(rb);

            if (isShoot) return;
            // Gây sát thương cho người chơi
            if (Object.HasStateAuthority && hit.GameObject.TryGetComponent<PlayerController>(out var hitPlayerController))
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
                hitPlayerController.TakeDamage_RPC(damage, hit.Point, hit.Normal, playerName);

                // Đảm bảo mũi tên không tiếp tục chịu ảnh hưởng vật lý sau va chạm
                //rb.isKinematic = true;

                // Căn chỉnh vị trí và hướng của mũi tên khi trúng người chơi
                transform.position = hit.Point - lastVelocity.normalized * penetrationDepth;
                transform.forward = lastVelocity.normalized;

                // Đặt mũi tên làm con của người chơi để nó dính vào người chơi
                transform.SetParent(hit.GameObject.transform);
            }
            else if (Object.HasStateAuthority && hit.GameObject.TryGetComponent<IDamageable>(out var hitBossNetworked))
            {
                
                string playerName = GameManager.Instance.PlayerData.playerName;
                hitBossNetworked.TakeDamage(damage, hit.Point, hit.Normal, playerName, NetworkManager.Instance.TeamID);
                isShoot = true;
                // Đảm bảo mũi tên không tiếp tục chịu ảnh hưởng vật lý sau va chạm
                //rb.isKinematic = true;

                // Căn chỉnh vị trí và hướng của mũi tên khi trúng người chơi
                transform.position = hit.Point - lastVelocity.normalized * penetrationDepth;
                transform.forward = lastVelocity.normalized;
                if (hit.GameObject.tag == "BatEnemy")
                {
                    Runner.Despawn(Object);
                   *//* SmallBatEnemy smallBatEnemy = hit.GameObject.GetComponent<SmallBatEnemy>();
                   transform.SetParent(smallBatEnemy.ArrowPos);*//*
                }
                else
                {
                    // Đặt mũi tên làm con của người chơi để nó dính vào người chơi
                    transform.SetParent(hit.GameObject.transform.parent);
                }
               

            }
            else
            {
                // Đảm bảo mũi tên không tiếp tục chịu ảnh hưởng vật lý sau va chạm
                //rb.isKinematic = true;

                // Căn chỉnh vị trí và hướng của mũi tên khi trúng người chơi
                transform.position = hit.Point - lastVelocity.normalized * penetrationDepth;
                transform.forward = lastVelocity.normalized;

                // Đặt mũi tên làm con của người chơi để nó dính vào người chơi
                transform.SetParent(hit.GameObject.transform);
            }
        }*/

        private void OnTriggerEnter(Collider other)
        {
           
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = (transform.position - hitPoint).normalized;
            
            if (Object.HasStateAuthority && other.gameObject.TryGetComponent<PlayerController>(out var hitPlayerController))
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
                if (hitPlayerController.playerName == playerName)
                {
                    return;
                }
                /*GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
                Destroy(vfx, 1f);*/
                hitPlayerController.TakeDamage_RPC(damage, hitPoint, hitNormal, playerName);
                //Runner.Despawn(Object);
                SethitVFX_RPC(other.gameObject.transform.position);


            }
            else if (Object.HasStateAuthority && other.gameObject.TryGetComponent<IDamageable>(out var hitBossNetworked))
            {

                string playerName = GameManager.Instance.PlayerData.playerName;

                hitBossNetworked.TakeDamage(damage, hitPoint, hitNormal, playerName, NetworkManager.Instance.TeamID);
                /* GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
                 Destroy(vfx, 1f);*/
                //Runner.Despawn(Object);
                SethitVFX_RPC(other.gameObject.transform.position);

            }
            else if (Object.HasStateAuthority && other.gameObject.TryGetComponent<IEnvironmentInteractable>(out var hitIEnvironmentInteractable))
            {

                hitIEnvironmentInteractable.OnHitByWeapon();
                /* GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
                 Destroy(vfx, 1f);*/
                //Runner.Despawn(Object);
                SethitVFX_RPC(other.gameObject.transform.position);
            }

        }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void SethitVFX_RPC(Vector3 posititon)
        {
            GameObject vfx = Instantiate(hitVFX, posititon, Quaternion.identity);
            Destroy(vfx, 1f);
            if(Object.HasInputAuthority)
            {
                Runner.Despawn(Object);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            /*if (other.CompareTag("BowCenter"))
            {
                bowCollider.isTrigger = false;  // Khi mũi tên rời khỏi BowCenter, dừng trigger
            }*/
        }
    }
}
