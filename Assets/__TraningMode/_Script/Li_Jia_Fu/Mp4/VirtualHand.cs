/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TraningMode;

public class VirtualHand : MonoBehaviour
{
    public SphereCollider capsuleCollider;
    public Bow Bow;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider.enabled = false;
    }
    public float targetTime = 3.0f; // Mốc thời gian cần đạt
    private float elapsedTime = 0.0f; // Thời gian đã trôi qua
    // Update is called once per frame
    void Update()
    {
       
        // Tăng thời gian đếm lên mỗi khung hình
        elapsedTime += Time.deltaTime;

        if(elapsedTime >=12.3f )
        {
            speed = 0;
        }
        else if (elapsedTime > 11)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            if(!capsuleCollider.enabled)
            {
                capsuleCollider.enabled = true;
            }

        }
        // Kiểm tra nếu thời gian đạt đến mốc
        if (elapsedTime >= targetTime)
        {
            Bow.isPress = false;

            // Reset thời gian nếu muốn tiếp tục đếm lại
            elapsedTime = -155f;

            // Hoặc có thể dừng đếm bằng cách vô hiệu hóa script này
            // this.enabled = false;
            }
    
    }
}
*/