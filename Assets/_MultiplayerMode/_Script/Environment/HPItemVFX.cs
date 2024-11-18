using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPItemVFX : MonoBehaviour
{
    [SerializeField]
    float rotationSpeedX, rotationSpeedY, rotationSpeedZ;

   
    void Update()
    {
        transform.Rotate(rotationSpeedX, rotationSpeedY, rotationSpeedZ);
    }
}
