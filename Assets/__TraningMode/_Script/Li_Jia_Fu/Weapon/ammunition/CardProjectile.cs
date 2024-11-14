using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class CardProjectile : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject,5f);
    }
    [SerializeField] private GameObject projectile;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "target" || collision.gameObject.tag == "Ground")
        {
            // Lấy vị trí va chạm đầu tiên từ thông tin va chạm
            Vector3 contactPoint = collision.contacts[0].point;

            // Sinh ra đối tượng projectile tại vị trí va chạm
            GameObject cardhitVFX = Instantiate(projectile, contactPoint, Quaternion.identity);
            Destroy(cardhitVFX,20f);
            Destroy(gameObject);
        }
       

    }

}
