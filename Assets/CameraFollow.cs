using UnityEngine;

public class TrueSideScrollCamera : MonoBehaviour
{
    public Camera mainCamera;

    [Header("相机固定位置 (侧视)")]
    public float sideDistance = 12f;  // 相机离玩家的横向距离 (X轴)
    public float height = 3f;        // 相机的高度 (Y轴)
    public float zOffset = 0f;       // 相机相对于玩家前后的偏移 (Z轴)

    [Header("固定旋转角度")]
    public Vector3 fixedRotation = new Vector3(10f, -90f, 0f); // 关键：手动设置角度

    [Header("平滑度")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // 应用固定的旋转，不再让 LookAt 乱转
        mainCamera.transform.rotation = Quaternion.Euler(fixedRotation);
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // 1. 计算目标位置：X轴固定在侧面，Y轴固定高度，Z轴平滑跟随
        Vector3 targetPosition = new Vector3(sideDistance, height, transform.position.z + zOffset);

        // 2. 平滑移动
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, smoothSpeed);

        // 3. 每一帧都确保旋转角度不变 (防止其他脚本干扰)
        mainCamera.transform.rotation = Quaternion.Euler(fixedRotation);
    }
}