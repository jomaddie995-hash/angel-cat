using UnityEngine;
using System.Collections; // 新增：添加IEnumerator需要的命名空间

public class StepSoundTrigger : MonoBehaviour
{
    public float triggerAmount = 20f; // 这个触发器每次触发增加的数值
    private DecibelMeter decibelMeter;

    // 新增：音效相关配置
    public AudioClip touchSound; // 玩家触碰时播放的音效片段
    private AudioSource audioSource; // 音频播放组件

    // 新增：逐帧动画相关配置
    public Sprite[] animationSprites; // 逐帧动画的精灵序列（按播放顺序排列）
    public float frameRate = 10f; // 逐帧动画的帧率（每秒播放多少帧）
    public Transform animationSpawnPoint; // 动画播放的位置（物体旁的指定点，可在编辑器赋值）
    public float animationDuration = 1f; // 动画播放时长（自动销毁动画物体）
    // 可自定义上方偏移距离（在Inspector面板调整，默认0.5米，方便灵活修改）
    public float upOffsetDistance = 0.5f;
    private GameObject animationGameObject; // 承载逐帧动画的临时物体
    private SpriteRenderer animationSpriteRenderer; // 精灵渲染器，用于切换帧

    void Start()
    {
        // 初始化时，查找场景中是否有DecibelMeter组件
        decibelMeter = FindObjectOfType<DecibelMeter>();
        if (decibelMeter == null)
        {
            Debug.LogError("在场景中未找到DecibelMeter组件！请确保已添加。");
        }

        // 初始化音频组件
        InitAudioSource();
    }

    /// <summary>
    /// 初始化音频播放组件（自动添加AudioSource）
    /// </summary>
    private void InitAudioSource()
    {
        // 尝试获取物体上的AudioSource，没有则自动添加
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 音频配置（可根据需求调整）
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家（Tag为"Player"）
        if (other.CompareTag("Player"))
        {
            // 如果找到管理器，就调用增加方法
            if (decibelMeter != null)
            {
                decibelMeter.AddDecibels(triggerAmount);
            }
            // 新增：播放玩家触碰音效
            PlayTouchSound();
            // 新增：在物体旁播放逐帧动画
            PlayFrameAnimationAtObjectSide();
            // Debug.Log("播放脚步声"); 
        }
    }

    // 新增：鼠标左键点击物体触发逻辑（Unity内置检测方法）
    void OnMouseDown()
    {
        // 检测是否按下鼠标左键（0对应左键，1右键，2中键）
        if (Input.GetMouseButtonDown(0))
        {
            // 校验DecibelMeter是否存在
            if (decibelMeter != null)
            {
                // 调用增加分贝方法，传入触发数值
                decibelMeter.AddDecibels(triggerAmount);
                // 可选：打印日志，方便查看触发状态
                Debug.Log("鼠标左键点击触发！分贝值增加：" + triggerAmount);
            }
            else
            {
                Debug.LogWarning("DecibelMeter组件不存在，无法增加分贝值！");
            }
        }
    }

    /// <summary>
    /// 播放玩家触碰音效
    /// </summary>
    private void PlayTouchSound()
    {
        // 校验音效片段是否已赋值
        if (touchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(touchSound); // 播放单次音效，不影响其他音频播放
        }
        else
        {
            if (touchSound == null)
            {
                Debug.LogWarning("请在Inspector面板为touchSound赋值音效片段！");
            }
        }
    }

    /// <summary>
    /// 在物体上方播放逐帧动画（已实现悬浮上方+50%缩小后再放大15%）
    /// </summary>
    private void PlayFrameAnimationAtObjectSide()
    {
        // 校验动画精灵序列是否为空
        if (animationSprites == null || animationSprites.Length == 0)
        {
            Debug.LogWarning("请在Inspector面板为animationSprites赋值精灵序列！");
            return;
        }

        // 销毁上一次可能残留的动画物体
        if (animationGameObject != null)
        {
            Destroy(animationGameObject);
        }

        // 1. 创建临时游戏物体用于承载逐帧动画
        animationGameObject = new GameObject("TouchFrameAnimation");
        // 2. 设置动画物体的位置（优先使用指定spawnPoint，否则悬浮在当前物体上方）
        if (animationSpawnPoint != null)
        {
            animationGameObject.transform.position = animationSpawnPoint.position;
            animationGameObject.transform.rotation = animationSpawnPoint.rotation;
        }
        else
        {
            // 悬浮在挂载脚本物体的上方（使用transform.up获取物体自身上方）
            animationGameObject.transform.position = transform.position + transform.up * upOffsetDistance;
            animationGameObject.transform.rotation = transform.rotation;
        }

        // 核心修改：原有50%缩放基础上，再放大15%（计算逻辑：0.5 * 1.15 = 0.575）
        // 1.15 对应15%的放大比例，保持等比缩放，避免动画变形
        animationGameObject.transform.localScale = Vector3.one * 0.575f;

        // 3. 添加精灵渲染器组件，用于显示逐帧精灵
        animationSpriteRenderer = animationGameObject.AddComponent<SpriteRenderer>();
        // 设置精灵排序（可根据需求调整sortingOrder）
        animationSpriteRenderer.sortingOrder = 5;

        // 4. 启动协程播放逐帧动画
        StartCoroutine(PlayFrameAnimationCoroutine());
    }

    /// <summary>
    /// 协程：逐帧切换精灵，实现逐帧动画
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayFrameAnimationCoroutine()
    {
        // 计算每帧间隔时间
        float frameInterval = 1f / frameRate;
        int currentFrameIndex = 0;

        // 循环切换精灵，直到播放完所有帧或达到动画时长
        float elapsedTime = 0f;
        while (currentFrameIndex < animationSprites.Length && elapsedTime < animationDuration)
        {
            // 切换当前帧的精灵
            animationSpriteRenderer.sprite = animationSprites[currentFrameIndex];
            // 切换到下一帧
            currentFrameIndex++;
            // 等待帧间隔时间
            yield return new WaitForSeconds(frameInterval);
            // 累计播放时间
            elapsedTime += frameInterval;
        }

        // 动画播放完毕后，销毁临时动画物体
        if (animationGameObject != null)
        {
            Destroy(animationGameObject);
        }
    }
}