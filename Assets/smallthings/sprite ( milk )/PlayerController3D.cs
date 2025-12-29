using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 10f;
    public float jumpForce = 7f;

    [Header("状态检测")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private BoxCollider col;
    private bool isGrounded;
    private bool jumpRequest = false; // 新增：用于记录跳跃指令

    private float originalHeight;
    private Vector3 originalCenter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();

        originalHeight = col.size.y;
        originalCenter = col.center;

        // 冻结旋转和X轴。开启 Interpolate 解决相机跟随抖动
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // 1. 地面检测
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);

        // 2. 捕捉跳跃输入 (不要在Update里直接改velocity，而是设个布尔值)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }

        // 3. 下蹲检测 (保持在Update以保证响应速度)
        HandleCrouch();
    }

    void FixedUpdate()
    {
        // 4. 统一处理速度
        // 先获取当前的水平/垂直速度
        float currentYVelocity = rb.velocity.y;

        // 如果有跳跃请求，修改 Y 轴速度
        if (jumpRequest)
        {
            currentYVelocity = jumpForce;
            jumpRequest = false; // 处理完后重置
        }

        // 统一应用速度：Z轴保持恒速，Y轴保持物理计算后的速度（或跳跃速度）
        rb.velocity = new Vector3(0, currentYVelocity, moveSpeed);
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.S))
        {
            col.size = new Vector3(col.size.x, originalHeight * 0.5f, col.size.z);
            col.center = new Vector3(originalCenter.x, originalCenter.y - (originalHeight * 0.25f), originalCenter.z);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            col.size = new Vector3(col.size.x, originalHeight, col.size.z);
            col.center = originalCenter;
        }
    }
}