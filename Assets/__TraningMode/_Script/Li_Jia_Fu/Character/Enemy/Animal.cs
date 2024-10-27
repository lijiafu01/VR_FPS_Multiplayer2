using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{

    public float speed = 5f; // Tốc độ di chuyển

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 10f;
        if (rb == null)
        {
            Debug.LogError("No Rigidbody attached to the object.");
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
    }
}
