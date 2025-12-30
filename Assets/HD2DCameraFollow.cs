using UnityEngine;

public class HD2DCameraFollow : MonoBehaviour
{
    [Header("追踪目标")]
    public Transform target; // 你的角色 Transform

    [Header("偏移与角度")]
    public Vector3 offset = new Vector3(0, 10, -10); // 摄像机相对于角色的距离
    public float smoothSpeed = 0.125f; // 跟随的平滑度（值越小越平滑）

    [Header("锁定轴向")]
    public bool lockY = false; // 如果是纯横向卷轴，可以锁定Y轴

    void LateUpdate()
    {
        if (target == null) return;

        // 1. 计算目标位置
        Vector3 desiredPosition = target.position + offset;

        // 2. 如果需要锁定 Y 轴（防止角色跳跃时摄像机上下晃动剧烈）
        if (lockY)
        {
            desiredPosition.y = transform.position.y;
        }

        // 3. 使用 Lerp 进行平滑插值
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 4. 应用位置
        transform.position = smoothedPosition;

        // 5. 始终盯着目标（或者你可以注释掉这一行，手动在 Inspector 设置固定旋转）
        // transform.LookAt(target);
    }
}