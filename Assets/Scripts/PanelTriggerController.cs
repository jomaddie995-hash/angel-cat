using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 引入协程命名空间，用于实现渐变延时

public class PanelTriggerController : MonoBehaviour
{
    // 拖入你的FullscreenPanel预制体（后续步骤会制作）
    public GameObject fullscreenPanelPrefab;
    // 标记是否已触发，防止重复弹出Panel
    private bool hasTriggered = false;
    // 渐显时长（可在编辑器调整，默认1秒完成渐显）
    public float fadeInDuration = 1f;
    [Header("音效设置")]
    public AudioSource bgmSource;    // 拖入场景中播放BGM的物体
    public AudioClip victoryMusic;   // 拖入胜利音效文件

    /// <summary>
    /// 触发器进入检测（Player进入时调用）
    /// </summary>
    /// <param name="other">进入触发器的碰撞体</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // --- 添加切换音效的代码 ---
            if (bgmSource != null && victoryMusic != null)
            {
                bgmSource.Stop();            // 停止原来的音乐
                bgmSource.clip = victoryMusic; // 换成胜利音效
                bgmSource.loop = false;      // 胜利音效通常不需要循环
                bgmSource.Play();            // 开始播放
            }
            // -------------------------

            StartCoroutine(ShowFullscreenPanelWithFadeIn());
        }

        // 检测是否是Player标签，且未触发过
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // 标记为已触发
            StartCoroutine(ShowFullscreenPanelWithFadeIn()); // 启动协程实现渐显
        }
    }

    /// <summary>
    /// 协程：带渐显效果显示全屏Panel+暂停游戏
    /// </summary>
    /// <returns>协程迭代器</returns>
    private IEnumerator ShowFullscreenPanelWithFadeIn()
    {
        // 1. 实例化全屏Panel（直接在场景中生成，继承Canvas的层级）
        GameObject panel = Instantiate(fullscreenPanelPrefab);
        // 2. 将Panel设置为Canvas的子对象（确保UI层级正确，避免显示异常）
        Transform canvasTransform = GameObject.Find("Canvas").transform;
        panel.transform.SetParent(canvasTransform, false);

        // 3. 获取或添加Canvas Group组件（用于控制透明度和交互）
        CanvasGroup panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null) // 如果Panel没有Canvas Group，自动添加
        {
            panelCanvasGroup = panel.AddComponent<CanvasGroup>();
        }

        // 4. 初始化Panel透明度为0（完全透明，不可见）
        panelCanvasGroup.alpha = 0f;
        // 可选：初始时禁止Panel交互（渐显完成后再开启，避免误操作）
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        // 5. 平滑渐显逻辑：根据时长逐步提升透明度
        float elapsedTime = 0f; // 已流逝的时间
        while (elapsedTime < fadeInDuration)
        {
            // 计算当前透明度（0 -> 1 的线性插值）
            elapsedTime += Time.unscaledDeltaTime; // 注意：游戏已暂停，需使用unscaledDeltaTime
            float targetAlpha = Mathf.Clamp01(elapsedTime / fadeInDuration); // 确保透明度在0~1之间
            panelCanvasGroup.alpha = targetAlpha;

            yield return null; // 等待下一帧，实现平滑过渡
        }

        // 6. 确保最终透明度为1（完全不透明）
        panelCanvasGroup.alpha = 1f;
        // 开启Panel交互（允许点击按钮等操作）
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        // 7. 暂停游戏时间（Time.timeScale=0时，游戏逻辑暂停，但UI不受影响）
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 结束游戏（供按钮绑定调用）
    /// </summary>
    public void QuitGame()
    {
        // 恢复游戏时间（可选，避免退出前时间处于暂停状态）
        Time.timeScale = 1f;
        // 编辑器中停止运行，打包后退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}