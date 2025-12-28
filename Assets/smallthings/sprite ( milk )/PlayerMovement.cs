using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 8f;
    public float jumpForce = 6f;

    private Rigidbody rb;
    private Animator anim;
    private bool isGrounded = true;
    private bool isFacingPositive = true;

    private float moveX;
    private float moveZ;
    private bool jumpRequested;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 获取当前物体或子物体上的动画机
        anim = GetComponentInChildren<Animator>();

        // 物理系统配置，解决抖动和缓慢
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.drag = 0f;
    }

    void Update()
    {
        // 1. 初始化输入
        moveX = 0f;
        moveZ = 0f;

        // --- 按照你要求的【反向】逻辑 ---
        if (Input.GetKey(KeyCode.A)) moveX = 1f;       // A 向正方向 (+X)
        else if (Input.GetKey(KeyCode.D)) moveX = -1f;  // D 向负方向 (-X)

        if (Input.GetKey(KeyCode.S)) moveZ = 1f;       // S 向前 (+Z)
        else if (Input.GetKey(KeyCode.W)) moveZ = -1f;  // W 向后 (-Z)

        // --- 2. 动画控制逻辑（同时更新 Speed 和 isMoving） ---
        if (anim != null)
        {
            // 判断当前是否有按键输入
            bool moving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f);

            if (moving)
            {
                anim.SetFloat("Speed", 1f);
                anim.SetBool("isMoving", true);  // 激活移动状态
            }
            else
            {
                anim.SetFloat("Speed", 0f);
                anim.SetBool("isMoving", false); // 强制回到静止状态
            }
        }

        // 3. 跳跃输入
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }

        // 4. 处理镜像
        HandleMirroring();
    }

    void FixedUpdate()
    {
        // 应用物理位移
        rb.velocity = new Vector3(moveX * moveSpeed, rb.velocity.y, moveZ * moveSpeed);

        // 执行跳跃
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpRequested = false;
        }
    }

    private void HandleMirroring()
    {
        if (moveX > 0 && !isFacingPositive) SetRotation(true);
        else if (moveX < 0 && isFacingPositive) SetRotation(false);
    }

    private void SetRotation(bool facePositive)
    {
        isFacingPositive = facePositive;
        // 角度设置：根据你的模型朝向进行 0 和 180 的切换
        transform.rotation = facePositive ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    void OnCollisionStay(Collision collision)
    {
        // 这里的 Tag 必须在 Unity Inspector 里给地面物体勾选上
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}