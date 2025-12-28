using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // 确保物体一定有 Rigidbody
public class NPCChase3D : MonoBehaviour
{
    [Header("追踪设置")]
    public string playerTag = "Player";   // 玩家的标签
    public float moveSpeed = 5f;          // 移动速度
    public float stopDistance = 1.2f;    // 停止距离

    [Header("物理表现")]
    public bool freezeZAxis = true;       // 如果是横版 3D，建议勾选，防止 NPC 跑到 Z 轴深处

    private Transform player;
    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始化物理约束：防止 NPC 摔倒
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        if (freezeZAxis)
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionZ;
        }

        // 寻找玩家
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError($"找不到标签为 {playerTag} 的物体，请检查玩家的 Tag 设置！");
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 计算水平方向向量 (只关注 X 轴)
        float directionX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(directionX);

        if (distance > stopDistance)
        {
            float moveDir = directionX > 0 ? 1 : -1;

            // 关键：保留当前的 Y 轴速度（重力），只改变水平速度
            Vector3 targetVelocity = new Vector3(moveDir * moveSpeed, rb.velocity.y, 0);

            // 使用线性插值让移动更平滑，减少由于物理摩擦产生的抖动
            rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, 0.5f);

            Flip(moveDir);
        }
        else
        {
            // 到达距离，水平速度清零，保留重力
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void Flip(float dir)
    {
        if (spriteRenderer == null) return;

        if (dir > 0)
            spriteRenderer.flipX = false;
        else if (dir < 0)
            spriteRenderer.flipX = true;
    }
}