using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TraningMode;
public class DummyTarget : MonoBehaviour
{
    public Animator anim;
    //target status
    public float moveSpeed = 5.0f; // Tốc độ di chuyển
    public float rotateSpeed = 90.0f; // Tốc độ quay quanh trục Y (độ/giây)
    public bool isMoving; // Kiểm soát việc di chuyển
    public bool isRotating; // Kiểm soát việc quay
    public bool isMovingAndRotating; // Kiểm soát việc vừa quay vừa di chuyển
    private Vector3 startPosition; // Lưu vị trí khởi đầu để tính toán di chuyển
    void Start()
    {
        startPosition = transform.position; // Cập nhật điểm khởi đầu từ vị trí hiện tại của đối tượng
    }
    void Update()
    {
        if (isMovingAndRotating)
        {
            HandleMovingAndRotating();
        }
        else
        {
            if (isMoving)
            {
                HandleMovement();
            }

            if (isRotating)
            {
                HandleRotation();
            }
        }
    }
    private void HandleMovement()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

    }
   
    private void HandleRotation()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);
    }
    private void HandleMovingAndRotating()
    {
        float angle = Mathf.Atan2(transform.position.z, transform.position.x) * Mathf.Rad2Deg;
        angle += moveSpeed * Time.deltaTime;
        float radius = Vector3.Distance(startPosition, Vector3.zero);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        transform.position = new Vector3(x, transform.position.y, z);
        HandleRotation();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "bullet")
        {
            VFXManager.Instance.WoodHitBig(collision);
            StartCoroutine(WaitForAnimation("hit"));
            Destroy(collision.gameObject);  
        }
    }
    public void GrenadeCollider()
    {
        StartCoroutine(WaitForAnimation("hit"));
    }

    IEnumerator WaitForAnimation(string stateName)
    {
        anim.SetTrigger(stateName);
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null; 
        }
        while (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        TrainingMission trainingMission = transform.GetComponentInParent<TrainingMission>();
        trainingMission.UpdateMissionProgress(1);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "objline")
        {
            moveSpeed = -moveSpeed;
        }
    }

}
