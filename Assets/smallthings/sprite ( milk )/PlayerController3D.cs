using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 10f;
    public float jumpForce = 7f;
    [Header("音效设置")]
    public AudioSource audioSource; // 拖入小猫身上的 AudioSource
    public AudioClip jumpSound;     // 跳跃音效
    public AudioClip crouchSound;   // 下蹲音效

    [Header("状态检测")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private BoxCollider col;
    private Animator anim; // 引用动画组件

    private bool isGrounded;
    private bool jumpRequest = false;

    private float originalHeight;
    private Vector3 originalCenter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        // HD-2D通常将Sprite和Animator放在子物体上，所以用GetComponentInChildren
        anim = GetComponentInChildren<Animator>();

        originalHeight = col.size.y;
        originalCenter = col.center;

        // 冻结旋转和X轴，开启插值平滑相机
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // 1. 地面检测
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);

        // 2. 捕捉跳跃输入
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
            // 2. 捕捉跳跃输入
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                jumpRequest = true;
                // --- 添加这一行 ---
                if (audioSource != null && jumpSound != null) audioSource.PlayOneShot(jumpSound);
            }

        }

        // 3. 处理下蹲
        HandleCrouch();

        // 4. 更新动画状态
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        float currentYVelocity = rb.velocity.y;

        if (jumpRequest)
        {
            currentYVelocity = jumpForce;
            jumpRequest = false;
        }

        // 应用位移：X轴锁定，Y轴物理计算，Z轴由moveSpeed决定
        rb.velocity = new Vector3(0, currentYVelocity, moveSpeed);
    }

    void HandleCrouch()
    {
        // 1. 按下 S 的瞬间：响一声音效
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (audioSource != null && crouchSound != null)
            {
                audioSource.PlayOneShot(crouchSound);
            }
        }

        // 2. 按住 S 键：执行下蹲逻辑
        if (Input.GetKey(KeyCode.S))
        {
            col.size = new Vector3(col.size.x, originalHeight * 0.5f, col.size.z);
            col.center = new Vector3(originalCenter.x, originalCenter.y - (originalHeight * 0.25f), originalCenter.z);

            if (anim != null) anim.SetBool("isCrouching", true);
        }
        // 3. 松开 S 键：恢复高度
        else if (Input.GetKeyUp(KeyCode.S))
        {
            col.size = new Vector3(col.size.x, originalHeight, col.size.z);
            col.center = originalCenter;

            if (anim != null) anim.SetBool("isCrouching", false);
        }
    }

    void UpdateAnimations()
    {
        if (anim == null) return;

        // 判断是否有 Z 轴速度来播放行走动画
        bool isMoving = Mathf.Abs(rb.velocity.z) > 0.1f;

        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);

        // 跳跃上升和下落的动画细节
        anim.SetFloat("vSpeed", rb.velocity.y);
    }
}
