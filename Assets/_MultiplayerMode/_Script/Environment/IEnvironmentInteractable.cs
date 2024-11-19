using UnityEngine;

public interface IEnvironmentInteractable
{
    void OnHitByWeapon();
    public void StaticEVM(Vector3 spawnPoint,Vector3 hitNormal);
}
