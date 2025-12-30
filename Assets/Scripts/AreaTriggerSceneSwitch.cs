using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AreaTriggerSceneSwitch : MonoBehaviour
{
    [Header("配置参数")]
    public GameObject triggerUI; // 拖拽赋值：需要触发显示的UI对象
    public string targetSceneName; // 目标场景名称（与Build Settings中一致）
    public float totalDelayTime = 1.5f; // 总延迟跳转时间（不变，仍为1.5秒）
    public float uiScaleTime = 0.5f; // UI放大动画时长（设置为0.5秒）
    [Header("触发配置：填写你创建的Tag名称")]
    public string targetObjectTag = "TriggerObject"; // 指定触发物体的Tag名称
    private bool hasTriggered = false; // 防止重复触发的标记
    private bool isUiScaling = false; // UI是否正在播放放大动画
    private float currentScaleTime = 0f; // 记录UI放大动画已播放时间

    void Start()
    {
        // 初始化UI状态：缩放为0（完全缩小，不可见）
        if (triggerUI != null)
        {
            triggerUI.SetActive(true);
            triggerUI.transform.localScale = Vector3.zero; // 初始缩放为0
        }
    }

    // 3D场景专用：触发器检测（添加Tag判断）
    private void OnTriggerEnter(Collider other)
    {
        // 只有：未触发过 + 进入物体的Tag与指定Tag一致 才会执行
        if (!hasTriggered && other.CompareTag(targetObjectTag))
        {
            TriggerUIScaleAndSceneSwitch();
        }
    }

    // 2D场景专用：若使用2D物体，替换上面的OnTriggerEnter为该方法
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (!hasTriggered && other.CompareTag(targetObjectTag))
    //     {
    //         TriggerUIScaleAndSceneSwitch();
    //     }
    // }

    /// <summary>
    /// 启动UI放大动画 + 延迟跳转场景
    /// </summary>
    private void TriggerUIScaleAndSceneSwitch()
    {
        hasTriggered = true; // 标记为已触发，防止重复执行
        isUiScaling = true; // 标记UI开始播放放大动画
        StartCoroutine(DelaySwitchScene()); // 启动协程，等待总时间后跳转场景
    }

    void Update()
    {
        // 播放UI放大动画：仅在触发后且动画未完成时执行
        if (isUiScaling && triggerUI != null)
        {
            // 累加动画时间
            currentScaleTime += Time.deltaTime;
            // 计算动画进度（0 ~ 1），确保不超过1
            float scaleProgress = Mathf.Clamp01(currentScaleTime / uiScaleTime);

            // 平滑插值缩放：从(0,0,0)到(1,1,1)
            triggerUI.transform.localScale = Vector3.Lerp(
                Vector3.zero, // 初始缩放
                Vector3.one,  // 目标缩放
                scaleProgress // 插值进度
            );

            // 当动画进度达到1时，停止缩放动画
            if (scaleProgress >= 1f)
            {
                isUiScaling = false;
                currentScaleTime = 0f; // 重置动画时间
            }
        }
    }

    /// <summary>
    /// 协程：延迟指定总时间后跳转场景
    /// </summary>
    private IEnumerator DelaySwitchScene()
    {
        yield return new WaitForSeconds(totalDelayTime); // 等待总时间1.5秒

        // 跳转场景（两种方式二选一）
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        // SceneManager.LoadScene(1); // 方式2：通过场景索引跳转
    }
}