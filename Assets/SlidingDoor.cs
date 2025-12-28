using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("设置")]
    public Vector3 openOffset = new Vector3(2f, 0, 0);
    public float speed = 3f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private BoxCollider doorPhysicsCollider; // 门本身的物理碰撞体

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition;

        // 获取门身上的 Collider（注意：不是触发器那个Cube，是门板上的）
        doorPhysicsCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        // 平滑移动逻辑
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("开门");
            targetPosition = closedPosition + openOffset;

            // 关键：门开始打开时，禁用门的物理碰撞，防止它把猫“推走”或“卡住”
            // if (doorPhysicsCollider != null) doorPhysicsCollider.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("关门");
            
            targetPosition = closedPosition;

            // 关键：门完全关上或开始关上时，恢复物理碰撞
            // 如果想更高级一点，可以用协程等门完全归位再恢复，但这里先简单处理
            // if (doorPhysicsCollider != null) doorPhysicsCollider.enabled = true;
        }
    }
}
