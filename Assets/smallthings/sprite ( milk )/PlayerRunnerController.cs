using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRunnerController : MonoBehaviour
{
    [Header("移动设置")]
    public float laneDistance = 3f;   // 赛道之间的距离
    public float moveSpeed = 10f;     // 左右切换赛道速度
    public float jumpForce = 8f;      // 跳跃力度

    [Header("物理检测")]
    public Transform groundCheck;     // 在脚下放一个空物体用于检测地面
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator anim;
    private int targetLane = 1;       // 0: 左, 1: 中, 2: 右
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        // 锁定旋转，防止跑酷时摔倒
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // 1. 地面检测
        isGrounded = Physics.OverlapSphere(groundCheck.position, groundRadius, groundLayer).Length > 0;
        anim.SetBool("isGrounded", isGrounded);

        // 2. 左右赛道切换 (Z 轴)
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (targetLane > 0) targetLane--;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (targetLane < 2) targetLane++;
        }

        // 3. 跳跃逻辑 (Y 轴)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("jump");
        }

        // 4. 蹲下逻辑 (S 键)
        if (Input.GetKey(KeyCode.S) && isGrounded)
        {
            anim.SetBool("isCrouching", true);
            // 这里可以缩短 Collider 的高度，以便钻过障碍物
        }
        else
        {
            anim.SetBool("isCrouching", false);
        }

        MoveToLane();
    }

    void MoveToLane()
    {
        // 根据 targetLane 计算目标 Z 坐标
        // 假设 中间赛道 Z = 0，左边 Z = -laneDistance，右边 Z = laneDistance
        float targetZ = (targetLane - 1) * laneDistance;

        Vector3 newPos = transform.position;
        // 使用 Lerp 平滑移动 Z 轴
        newPos.z = Mathf.Lerp(newPos.z, targetZ, Time.deltaTime * moveSpeed);

        transform.position = newPos;
    }

    // 在编辑器里画出地面检测范围
    // 在编辑器里画出地面检测范围
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            // 注意：D 必须大写
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}