using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TraningMode;
public class Bullet : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject,5f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
    /* private void OnEnable()
     {
         Invoke("ReturnObjectPool", 5f);
     }
    *//* private void ReturnObjectPool()
     {
         Destroy(gameObject);
         //ObjectPoolManager.Instance.ReturnToPool("pistolbullet", transform.gameObject);
     }*/
}
