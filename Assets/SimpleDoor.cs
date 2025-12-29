using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    public Transform doorPivot; // 这里等下拖入 New_Door_Group
    public float openAngle = -90f; // 开门度数
    public float closeAngle = 0f;
    public float speed = 3f;

    private bool playerIn = false;

    void Update()
    {
        // 确定目标旋转角度
        float targetY = playerIn ? openAngle : closeAngle;
        Quaternion targetRot = Quaternion.Euler(0, targetY, 0);

        // 平滑旋转
        doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, targetRot, Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIn = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIn = false;
    }
}
