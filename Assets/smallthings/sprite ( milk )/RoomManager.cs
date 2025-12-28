using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomPackage
{
    public string roomName;
    public Collider triggerZone;
    public GameObject roomContent;
}

public class RoomManager : MonoBehaviour
{
    public List<RoomPackage> roomList = new List<RoomPackage>();

    void Start()
    {
        // 游戏开始时，为了保险，可以先隐藏所有房间
        // 或者你可以手动在编辑器里留下初始房间，关闭其他房间
    }

    // 当玩家进入某个房间的触发区域时
    public void HandleEnter(Collider zone)
    {
        foreach (var package in roomList)
        {
            if (package.roomContent == null) continue;

            // 核心逻辑：
            // 如果这个包里的 triggerZone 是玩家刚刚踩到的那个，就显示 (true)
            // 否则，全部隐藏 (false)
            if (package.triggerZone == zone)
            {
                package.roomContent.SetActive(true);
            }
            else
            {
                package.roomContent.SetActive(false);
            }
        }
    }

    // 注意：在这种“排他性”逻辑下，我们通常不需要 HandleExit。
    // 因为进入下一个房间的 Enter 事件会自动关闭上一个房间。
}