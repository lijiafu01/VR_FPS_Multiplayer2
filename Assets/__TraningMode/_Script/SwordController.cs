using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public GameObject swordSlashPrefab;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch; // Controller bên phải

    public float startVelocityThreshold = 1.0f; // Ngưỡng bắt đầu cú vung
    public float endVelocityThreshold = 0.3f; // Ngưỡng kết thúc cú vung
    public float bufferTime = 0.5f; // Khoảng thời gian lưu trữ dữ liệu
    public float minSwingDuration = 0.1f; // Thời gian tối thiểu của cú vung
    public float maxSwingDuration = 1.0f; // Thời gian tối đa của cú vung

    public float slashSpeedMultiplier = 3.0f; // Hệ số nhân tốc độ kiếm khí
    public float minForceThreshold = 1.5f; // Lực tối thiểu để sinh ra kiếm khí

    private class MotionData
    {
        public Vector3 position;
        public Vector3 velocity;
        public float time;
    }

    private List<MotionData> motionBuffer = new List<MotionData>();
    private SwingState swingState = SwingState.Idle;
    private float swingStartTime;
    private Vector3 swingStartPosition;
    private Vector3 swingEndPosition;

    private float maxVelocityMagnitude = 0f; // Lưu trữ lực mạnh nhất
    private Vector3 maxVelocityDirection = Vector3.zero; // Hướng tại lực mạnh nhất

    private enum SwingState
    {
        Idle,
        Swinging
    }

    void Update()
    {
        // Lấy dữ liệu chuyển động
        Vector3 controllerPosition = transform.position;
        Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(controller);
        float currentTime = Time.time;

        // Thêm dữ liệu vào buffer
        motionBuffer.Add(new MotionData()
        {
            position = controllerPosition,
            velocity = controllerVelocity,
            time = currentTime
        });

        // Loại bỏ dữ liệu cũ
        while (motionBuffer.Count > 0 && currentTime - motionBuffer[0].time > bufferTime)
        {
            motionBuffer.RemoveAt(0);
        }

        // Phân tích cú vung kiếm
        switch (swingState)
        {
            case SwingState.Idle:
                if (controllerVelocity.magnitude > startVelocityThreshold)
                {
                    // Bắt đầu cú vung
                    swingState = SwingState.Swinging;
                    swingStartTime = currentTime;
                    swingStartPosition = controllerPosition;

                    // Khởi tạo giá trị cho vận tốc lớn nhất
                    maxVelocityMagnitude = controllerVelocity.magnitude;
                    maxVelocityDirection = controllerVelocity.normalized;
                }
                break;

            case SwingState.Swinging:
                // Cập nhật vận tốc lớn nhất
                if (controllerVelocity.magnitude > maxVelocityMagnitude)
                {
                    maxVelocityMagnitude = controllerVelocity.magnitude;
                    maxVelocityDirection = controllerVelocity.normalized;
                }

                if (controllerVelocity.magnitude < endVelocityThreshold)
                {
                    // Kết thúc cú vung
                    float swingDuration = currentTime - swingStartTime;

                    if (swingDuration >= minSwingDuration && swingDuration <= maxSwingDuration)
                    {
                        swingEndPosition = controllerPosition;

                        // Kiểm tra xem lực mạnh nhất có vượt qua ngưỡng tối thiểu không
                        if (maxVelocityMagnitude >= minForceThreshold)
                        {
                            // Hướng kiếm khí là trục Y của thanh kiếm
                            Vector3 swingDirection = transform.up; // Trục Y địa phương của thanh kiếm

                            // Tính lực dựa trên lực mạnh nhất
                            float swingForce = maxVelocityMagnitude;

                            // Tạo kiếm khí
                            SpawnSwordSlash(swingEndPosition, swingDirection, swingForce);
                        }
                    }

                    // Đặt lại trạng thái
                    swingState = SwingState.Idle;
                    maxVelocityMagnitude = 0f;
                    maxVelocityDirection = Vector3.zero;
                }
                break;
        }
    }

    void SpawnSwordSlash(Vector3 position, Vector3 direction, float force)
    {
        GameObject swordSlash = Instantiate(swordSlashPrefab, position, Quaternion.LookRotation(direction));
        Rigidbody rb = swordSlash.GetComponent<Rigidbody>();
        rb.velocity = direction.normalized * force * slashSpeedMultiplier;
    }
}
