using UnityEngine;

public class HD2D_CameraFollow : MonoBehaviour
{
    [Header("追踪目标")]
    public Transform target; // 在 Inspector 中拖入 Player

    [Header("平滑参数")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f; // 数值越小越平滑

    [Header("偏移位置")]
    public Vector3 offset = new Vector3(0, 8, -10); // HD-2D 经典的斜上方视角

    // 使用 LateUpdate 确保在角色移动后再更新相机位置，防止画面抖动
    private void LateUpdate()
    {
        if (target == null) return;

        // 1. 计算目标点（玩家当前坐标 + 预设偏移量）
        Vector3 desiredPosition = target.position + offset;

        // 2. 使用 Lerp（线性插值）让相机平滑地滑向目标点
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. 应用位置
        transform.position = smoothedPosition;

        // 注意：HD-2D 通常保持固定旋转角度，不建议使用 LookAt
        // 如果需要调整角度，请直接在 Camera 的 Transform 中旋转
    }
}