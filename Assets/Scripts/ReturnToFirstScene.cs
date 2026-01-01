using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 引入UI命名空间（可选，确保兼容按钮组件）

/// <summary>
/// 挂载到按钮上，按下后返回Build Settings首个场景并重置游戏
/// </summary>
[RequireComponent(typeof(Button))] // 自动要求挂载Button组件，防止误挂
public class ReturnToFirstScene : MonoBehaviour
{
    /// <summary>
    /// 游戏重置并返回初始场景（供按钮绑定调用）
    /// </summary>
    public void ResetGameToFirstScene()
    {
        // 1. 恢复游戏时间（关键：防止游戏处于暂停状态导致场景加载异常）
        Time.timeScale = 1f;

        // 2. 停止所有正在播放的协程（可选，清理残留逻辑）
        StopAllCoroutines();

        // 3. 获取Build Settings中的首个场景（索引为0的场景即最开始的场景）
        if (SceneManager.sceneCountInBuildSettings > 0)
        {
            // 加载首个场景（使用LoadSceneMode.Single确保销毁当前所有场景对象，彻底重置）
            SceneManager.LoadScene(0, LoadSceneMode.Single);

            Debug.Log("游戏已重置，正在返回初始场景！");
        }
        else
        {
            Debug.LogError("Build Settings中未添加任何场景，请先配置场景！");
        }
    }
}