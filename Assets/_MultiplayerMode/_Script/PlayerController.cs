using Fusion;
using OculusSampleFramework;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
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

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            hardwareRig = FindObjectOfType<HardwareRig>();
            Debug.Log("dev1_tim thay hardwareRig " + hardwareRig.gameObject.name);
            _currentHp = _maxHp; // Đặt HP ban đầu khi player được spawn
        }

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = _fireSound;
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
        Debug.Log("dev_fire_weapon");
        _weaponHandler.Fire(isRight);

        // Gọi hàm để chơi âm thanh và hiển thị hiệu ứng tóe lửa
        FireWeaponEffects_RPC();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void FireWeaponEffects_RPC()
    {
        Debug.Log("dev_fire_weapon_effects_rpc");
        if (_audioSource != null && _fireSound != null)
        {
            _audioSource.Play();
            Debug.Log("dev_fire_sound_played");
        }

        if (isRight)
        {
            _muzzleFlash.Play();
            Debug.Log("dev_muzzle_flash_played");
            Invoke(nameof(StopMuzzleFlash), 1f); // Dừng hiệu ứng sau 1 giây

        }
        else
        {
            
            _leftMuzzleFlash.Play();
            Invoke(nameof(StopMuzzleFlash), 1f); // Dừng hiệu ứng sau 1 giây
        }
    }

    private void StopMuzzleFlash()
    {
        if(isRight)
        {
            _muzzleFlash.Stop();
            Debug.Log("dev_muzzle_flash_stopped");
        }
        if (_muzzleFlash != null)
        {
            _leftMuzzleFlash.Stop();
            Debug.Log("dev_muzzle_flash_stopped");
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

        Debug.Log("dev3_player moved to new position: " + positions[randomIndex]);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TakeDamage_RPC(int damage, Vector3 hitPosition, Vector3 hitNormal)
    {
        Debug.Log("dev_take_damage_rpc_called");
        TakeDamage(damage, hitPosition, hitNormal);
    }

    public void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal)
    {
        Debug.Log("dev2_TakeDamage____" + Object.HasStateAuthority);
        if (Object.HasStateAuthority)
        {
            _currentHp -= damage;
            Debug.Log("dev2_Player HP: " + _currentHp);

            // Hiển thị hiệu ứng máu khi bị bắn
            PlayBloodEffect_RPC(hitPosition, hitNormal);

            if (_currentHp <= 0)
            {
                Dead();
                _currentHp = _maxHp; // Đặt lại HP khi player chết
            }
        }
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
