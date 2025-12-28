using UnityEngine;

public class TriggerProxy : MonoBehaviour
{
    private RoomManager manager;

    void Start()
    {
        // 自动在场景中寻找指挥官脚本
        manager = Object.FindFirstObjectByType<RoomManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 只有玩家进入时，才通知指挥官切换房间
        if (other.CompareTag("Player") && manager != null)
        {
            manager.HandleEnter(GetComponent<Collider>());
        }
    }

    // 删除了对 HandleExit 的调用，因为进入下一个房间会自动关闭上一个
    private void OnTriggerExit(Collider other)
    {
        // 这里留空，或者直接把这个函数删掉
    }
}