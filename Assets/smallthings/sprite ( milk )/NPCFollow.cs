using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public Transform player;          // 玩家的变换组件
    public float followDistance = 2.0f; // NPC 停止移动的距离（防止穿模）
    public float moveSpeed = 3.5f;    // NPC 的移动速度
    public bool isFollowing = false;  // 是否开启跟随模式

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // 如果没有手动拖入玩家，尝试通过 Tag 自动查找
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (isFollowing && player != null)
        {
            FollowPlayer();
        }
    }

    // 调用此方法开启跟随（可以由交互脚本触发）
    public void StartFollowing()
    {
        isFollowing = true;
        Debug.Log("NPC 开始跟随你了！");
    }

    void FollowPlayer()
    {
        // 计算 NPC 与玩家之间的距离
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            // 1. 转向玩家
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // 2. 向玩家移动
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // 3. 播放移动动画 (复用你之前的 Speed 参数逻辑)
            if (anim != null) anim.SetFloat("Speed", 1.0f);
        }
        else
        {
            // 距离够近了，停下播放静止动画
            if (anim != null) anim.SetFloat("Speed", 0f);
        }
    }
}