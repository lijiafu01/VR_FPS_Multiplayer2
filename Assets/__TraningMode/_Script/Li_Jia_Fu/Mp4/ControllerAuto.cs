/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerAuto : MonoBehaviour
{
    public Pistol pistol;
    public float speed;
    public Animator Animator;
    public float targetTime = 3.0f; // Mốc thời gian cần đạt
    private float elapsedTime = 0.0f; // Thời gian đã trôi qua
    // Update is called once per frame
    private void Start()
    {
        Animator.SetFloat("Grip", 0);

    }
    void Update()
    {
        // Tăng thời gian đếm lên mỗi khung hình
        elapsedTime += Time.deltaTime;

        if (elapsedTime > 12f)
        {
            speed = 0;
        }
        else if(elapsedTime > 11)
        {
            
                {
                transform.position += transform.forward * speed * Time.deltaTime;


            }
        }
        // Kiểm tra nếu thời gian đạt đến mốc
        if (elapsedTime >= targetTime)
        {
            Animator.SetFloat("Trigger", 0);

            // Reset thời gian nếu muốn tiếp tục đếm lại
            elapsedTime = -55f;

            // Hoặc có thể dừng đếm bằng cách vô hiệu hóa script này
            // this.enabled = false;
        }
        else if (elapsedTime > 12.8f)
        {
            Animator.SetFloat("Trigger", 1);
            pistol.Fire();
            pistol.isStop = false;
        }

    }
}
*/