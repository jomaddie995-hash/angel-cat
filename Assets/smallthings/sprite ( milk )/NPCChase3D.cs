using UnityEngine;

public class NPCChase3D : MonoBehaviour
{
    [Header("追击目标")]
    public Transform player;
    public float speedOffset = -0.2f; // 比玩家慢 0.2 单位，增加容错
    public float catchDistance = 1.2f; // 判定抓到的距离

    private Rigidbody rb;
    private PlayerController3D playerScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player != null)
            playerScript = player.GetComponent<PlayerController3D>();

        // 同样锁定旋转和X轴
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 1. 获取玩家当前速度并同步到 Z 轴
        float currentChaseSpeed = playerScript.moveSpeed + speedOffset;
        rb.velocity = new Vector3(0, rb.velocity.y, currentChaseSpeed);

        // 2. 判定是否追上玩家
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < catchDistance)
        {
            Debug.Log("玩家被 NPC 抓住了！游戏结束");
            // 这里可以添加 Time.timeScale = 0; 来暂停游戏
        }
    }
}