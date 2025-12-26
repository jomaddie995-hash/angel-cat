using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded;
    // --- 新增：声明动画组件变量 ---
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // --- 新增：获取动画组件 ---
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float xMove = 0f;
        float zMove = 0f;

        if (Input.GetKey(KeyCode.S)) zMove = moveSpeed;
        if (Input.GetKey(KeyCode.W)) zMove = -moveSpeed;
        if (Input.GetKey(KeyCode.D)) xMove = -moveSpeed;
        if (Input.GetKey(KeyCode.A)) xMove = moveSpeed;

        Vector3 velocity = rb.velocity;
        velocity.x = xMove;
        velocity.z = zMove;
        rb.velocity = velocity;

        // --- 新增：处理动画逻辑 ---
        // 只要 xMove 或 zMove 不为 0，就代表正在移动
        bool isMoving = (xMove != 0 || zMove != 0);

        // 只有当动画组件存在时才赋值，防止报错
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}