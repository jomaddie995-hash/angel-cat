using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossTriggerGameReset : MonoBehaviour
{
    // 可在编辑器拖拽赋值的引用（已替换预制体名称，避免冲突）
    [Header("核心配置")]
    public GameObject gameOverPanelPrefab; // 全屏Panel预制体（新名称：游戏结束Panel）
    public AudioClip triggerSound; // 触发时播放的音效文件（原触发音效）
    public AudioClip loseSound; // 新增：游戏失败（Lose）音效文件
    private AudioSource audioSource; // 音频播放组件
    private bool hasTriggered = false; // 防止重复触发标记
    public float fadeInDuration = 1f; // Panel渐显时长（默认1秒）

    [Header("BGM控制配置（新增）")]
    public GameObject bgmManagerObj; // 拖入BGM管理器所在游戏对象
    private AudioSource _bgmAudioSource; // BGM音频源缓存

    // 新增：若Boss使用CharacterController移动，需手动赋值（可选）
    [Header("Boss移动配置（按需赋值）")]
    public CharacterController bossCharacterController;

    void Start()
    {
        // 自动获取或添加AudioSource组件（无需手动挂载）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 音频配置：不循环播放，可根据需求调整音量
        audioSource.loop = false;
        audioSource.volume = 1f;

        // 自动获取CharacterController组件（若未手动赋值，尝试自动查找）
        if (bossCharacterController == null)
        {
            bossCharacterController = GetComponent<CharacterController>();
        }

        // 新增：初始化BGM音频源
        InitBGMSetting();
    }

    /// <summary>
    /// 新增：初始化BGM配置
    /// </summary>
    private void InitBGMSetting()
    {
        if (bgmManagerObj != null)
        {
            // 获取BGM管理器上的AudioSource组件
            _bgmAudioSource = bgmManagerObj.GetComponent<AudioSource>();
            if (_bgmAudioSource == null)
            {
                Debug.LogWarning("【BossTriggerGameReset】BGM管理器对象上未挂载AudioSource组件！");
            }
        }
        else
        {
            Debug.LogError("【BossTriggerGameReset】请为bgmManagerObj绑定BGM管理器游戏对象！");
        }
    }

    /// <summary>
    /// 碰撞进入检测（Boss与Player接触时触发）
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 检测双方标签、是否已触发、Panel预制体是否赋值
        if (other.CompareTag("Player") && gameObject.CompareTag("Boss") && !hasTriggered && gameOverPanelPrefab != null)
        {
            hasTriggered = true; // 标记为已触发，防止重复执行
            StopBossMovement(); // 新增：触碰后立即停止Boss移动
            PlayTriggerSound(); // 播放原触发音效
            PlayLoseSound(); // 新增：播放Lose音效
            StopBGM(); // 新增：播放Lose音效时关闭BGM
            StartCoroutine(ShowPanelWithFadeIn()); // 启动协程实现Panel渐显
        }
    }

    /// <summary>
    /// 新增：播放Lose音效
    /// </summary>
    private void PlayLoseSound()
    {
        // 空值判断，避免无效调用报错
        if (loseSound != null && audioSource != null)
        {
            // 停止当前可能正在播放的触发音效，再播放Lose音效
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            // 若需要Lose音效循环播放，可开启此行（默认不循环）
            // audioSource.loop = true;
            audioSource.clip = loseSound;
            audioSource.Play();
            Debug.Log("【BossTriggerGameReset】Lose音效已播放！");
        }
        else
        {
            if (loseSound == null)
            {
                Debug.LogWarning("【BossTriggerGameReset】请为loseSound绑定Lose音效文件！");
            }
        }
    }

    /// <summary>
    /// 新增：停止BGM播放（使用Pause保留播放进度，更友好）
    /// </summary>
    private void StopBGM()
    {
        // 空值判断，确保BGM音频源有效
        if (_bgmAudioSource != null && _bgmAudioSource.isPlaying)
        {
            _bgmAudioSource.Pause(); // 暂停BGM（保留播放位置，恢复时继续播放）
            // 若需要完全停止BGM（从头播放），可替换为 _bgmAudioSource.Stop();
            Debug.Log("【BossTriggerGameReset】BGM已暂停！");
        }
    }

    /// <summary>
    /// 新增：恢复BGM播放
    /// </summary>
    private void ResumeBGM()
    {
        // 空值判断，避免重复播放报错
        if (_bgmAudioSource != null && !_bgmAudioSource.isPlaying)
        {
            _bgmAudioSource.Play();
            Debug.Log("【BossTriggerGameReset】BGM已恢复播放！");
        }
    }

    /// <summary>
    /// 新增：停止Lose音效播放
    /// </summary>
    private void StopLoseSound()
    {
        if (audioSource != null && audioSource.clip == loseSound && audioSource.isPlaying)
        {
            audioSource.Stop();
            // 重置音效循环状态（若之前开启了循环）
            audioSource.loop = false;
            Debug.Log("【BossTriggerGameReset】Lose音效已停止！");
        }
    }

    /// <summary>
    /// 停止Boss移动的核心方法（兼容两种移动方式）
    /// </summary>
    private void StopBossMovement()
    {
        // 方案1：禁用Boss的刚体（适用于Rigidbody物理移动）
        Rigidbody bossRigidbody = GetComponent<Rigidbody>();
        if (bossRigidbody != null)
        {
            // 停止物理运动：清空速度 + 冻结刚体
            bossRigidbody.velocity = Vector3.zero;
            bossRigidbody.angularVelocity = Vector3.zero;
            bossRigidbody.isKinematic = true; // 冻结刚体，完全停止物理移动
        }

        // 方案2：禁用CharacterController（适用于角色控制器移动）
        if (bossCharacterController != null)
        {
            bossCharacterController.enabled = false; // 禁用控制器，停止移动
        }

        // 方案3：禁用所有移动相关脚本（适用于自定义脚本控制移动）
        // 查找并禁用所有名称包含“Move”/“Controller”的自定义移动脚本（可按需修改脚本名）
        MonoBehaviour[] moveScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in moveScripts)
        {
            if (script != this && (script.GetType().Name.Contains("Move") || script.GetType().Name.Contains("Controller")))
            {
                script.enabled = false;
            }
        }
    }

    /// <summary>
    /// 播放原触发音效
    /// </summary>
    private void PlayTriggerSound()
    {
        // 若音效文件已赋值，则单次播放音效（不干扰其他音频）
        if (triggerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(triggerSound);
        }
    }

    /// <summary>
    /// 协程：实现Panel平滑渐显
    /// </summary>
    private IEnumerator ShowPanelWithFadeIn()
    {
        // 1. 查找场景中的Canvas（UI根节点，确保Panel正常显示）
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("场景中未找到Canvas对象，请先创建UI Canvas！");
            yield break; // 终止协程，避免后续报错
        }

        // 2. 实例化全屏Panel
        GameObject panelInstance = Instantiate(gameOverPanelPrefab);
        // 挂载到Canvas下，保持UI缩放和层级正确
        panelInstance.transform.SetParent(canvas.transform, false);

        // 3. 获取或添加CanvasGroup组件（控制透明度实现渐显）
        CanvasGroup panelCanvasGroup = panelInstance.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = panelInstance.AddComponent<CanvasGroup>();
        }

        // 4. 初始化Panel状态（完全透明，不可交互）
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        // 5. 平滑渐显逻辑
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            // 使用unscaledDeltaTime，不受游戏时间缩放影响
            elapsedTime += Time.unscaledDeltaTime;
            // 计算目标透明度（限制在0~1之间）
            float targetAlpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            panelCanvasGroup.alpha = targetAlpha;
            yield return null; // 等待下一帧，实现平滑过渡
        }

        // 6. 确保Panel完全不透明，并开启交互（允许点击按钮）
        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        // 可选：暂停游戏（需要暂停则取消注释）
        // Time.timeScale = 0f;
    }

    /// <summary>
    /// 重新开始游戏（供Panel上的按钮绑定调用）- 已新增音效和BGM控制逻辑
    /// </summary>
    public void RestartGameScene()
    {
        // 新增：停止Lose音效播放
        StopLoseSound();

        // 新增：恢复BGM播放
        ResumeBGM();

        // 恢复游戏时间（若之前暂停了游戏，必须先恢复）
        Time.timeScale = 1f;

        // 获取当前场景名称，重新加载场景（回到初始状态）
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        // 触发标记重置（场景加载后脚本会重新初始化，此句可选）
        hasTriggered = false;
    }
}