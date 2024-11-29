using multiplayerMode;
using System.Collections;
using UnityEngine;
public class Portal : MonoBehaviour
{
    public AudioSource portalSFX;
    private PortalManager manager;
    private Coroutine collisionCoroutine;
    private bool isColliding = false;
    private void Start()
    {
        manager = GetComponentInParent<PortalManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<LocalPlayer>(out var hitLocalPlayer))
        {
            // Chỉ bắt đầu Coroutine nếu chưa có va chạm đang diễn ra
            if (!isColliding)
            {
                portalSFX.Play();
                isColliding = true;
                collisionCoroutine = StartCoroutine(CheckContinuousCollision(other, hitLocalPlayer));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Dừng Coroutine nếu đối tượng rời khỏi vùng va chạm
        if (collisionCoroutine != null)
        {
            portalSFX.Stop();
            StopCoroutine(collisionCoroutine);
            collisionCoroutine = null;
            isColliding = false;
        }
    }
    private IEnumerator CheckContinuousCollision(Collider other, LocalPlayer hitLocalPlayer)
    {
        float collisionTime = 0f;

        while (collisionTime < 2f)
        {
            // Nếu đối tượng rời khỏi vùng va chạm, dừng lại
            if (!other.bounds.Intersects(GetComponent<Collider>().bounds))
            {
                isColliding = false;
                yield break;
            }

            collisionTime += Time.deltaTime;
            yield return null;
        }
        // Gọi hàm teleport sau khi va chạm liên tục trong 2 giây
        manager.TeleportObject(this.transform, hitLocalPlayer.transform);
        isColliding = false;
    }
}
