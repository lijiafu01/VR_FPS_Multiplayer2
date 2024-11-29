using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PortalManager : MonoBehaviour
{
    // Danh sách các điểm Transform
    public List<Transform> portalPoints = new List<Transform>();
    // Hàm dịch chuyển đối tượng từ người gọi đến vị trí đích
    public void TeleportObject(Transform caller, Transform teleObject)
    {
        if (caller == null || teleObject == null)
        {
            Debug.LogError("Caller or teleObject is null. Teleportation aborted.");
            return;
        }
        if (portalPoints.Count == 0)
        {
            Debug.LogError("No portal points available for teleportation.");
            return;
        }
        // Tạo vị trí ngẫu nhiên không trùng với caller
        Transform chosenPoint;
        do
        {
            chosenPoint = portalPoints[Random.Range(0, portalPoints.Count)];
        } while (chosenPoint == caller && portalPoints.Count > 1);
        // Dịch chuyển đối tượng teleObject đến vị trí được chọn
        teleObject.position = chosenPoint.position;
        teleObject.rotation = chosenPoint.rotation;
    }
}
