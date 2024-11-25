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
   
}
