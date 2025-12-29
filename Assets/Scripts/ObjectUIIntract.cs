using UnityEngine;
using UnityEngine.UI;

public class ObjectUIIntract : MonoBehaviour
{
    // 配置参数（在Inspector面板赋值）
    public GameObject floatingTipUIPrefab; // 悬浮提示UI预制体（即FloatingTipUI）
    public Canvas operateCanvas; // 操作界面UI（即OperateCanvas）
    public float detectDistance = 3f; // 检测距离：玩家靠近该距离内显示悬浮UI
    public Vector3 uiOffset = new Vector3(0, 2, 0); // 悬浮UI相对于物体的偏移量（默认在物体上方2米）

    // 私有变量
    private GameObject spawnedTipUI; // 实例化后的悬浮UI对象
    private Transform playerTransform; // 玩家Transform组件
    private bool isInDetectRange; // 是否进入检测范围
    private bool isOperateUIActive; // 操作UI是否激活

    void Start()
    {
        // 查找玩家物体（确保玩家Tag为"Player"）
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("未找到Tag为Player的玩家物体！");
            return;
        }

        // 初始化操作UI为隐藏状态
        if (operateCanvas != null)
        {
            operateCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("请为operateCanvas赋值操作界面UI！");
        }

        // 初始化悬浮UI（先不实例化，进入范围后再创建）
        spawnedTipUI = null;
    }

    void Update()
    {
        if (playerTransform == null || floatingTipUIPrefab == null || operateCanvas == null)
        {
            return;
        }

        // 1. 计算玩家与目标物体的距离
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 2. 判断是否进入检测范围，控制悬浮UI的显示/隐藏
        if (distanceToPlayer <= detectDistance)
        {
            if (!isInDetectRange)
            {
                isInDetectRange = true;
                // 实例化悬浮UI，设置在物体附近
                SpawnFloatingTipUI();
            }

            // 3. 当悬浮UI显示时，按F键触发操作界面UI
            if (Input.GetKeyDown(KeyCode.F) && !isOperateUIActive)
            {
                ShowOperateCanvas();
                // 触发后隐藏悬浮UI
                if (spawnedTipUI != null)
                {
                    Destroy(spawnedTipUI);
                }
            }
        }
        else
        {
            if (isInDetectRange)
            {
                isInDetectRange = false;
                // 离开检测范围，销毁悬浮UI
                if (spawnedTipUI != null)
                {
                    Destroy(spawnedTipUI);
                }
            }
        }

        // 4. 当操作界面UI激活时，按任意键关闭
        if (isOperateUIActive && Input.anyKeyDown)
        {
            HideOperateCanvas();
        }
    }

    /// <summary>
    /// 实例化悬浮提示UI
    /// </summary>
    private void SpawnFloatingTipUI()
    {
        // 计算悬浮UI的生成位置（物体位置 + 偏移量）
        Vector3 uiSpawnPos = transform.position + uiOffset;
        // 实例化悬浮UI
        spawnedTipUI = Instantiate(floatingTipUIPrefab, uiSpawnPos, Quaternion.identity);
        // 让悬浮UI始终面向玩家（可选，优化显示效果）
        spawnedTipUI.transform.LookAt(playerTransform);
        spawnedTipUI.transform.rotation = Quaternion.Euler(0, spawnedTipUI.transform.rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// 显示操作界面UI
    /// </summary>
    private void ShowOperateCanvas()
    {
        isOperateUIActive = true;
        operateCanvas.gameObject.SetActive(true);
        // 可选：暂停游戏（若需要）
        // Time.timeScale = 0;
    }

    /// <summary>
    /// 隐藏操作界面UI
    /// </summary>
    private void HideOperateCanvas()
    {
        isOperateUIActive = false;
        operateCanvas.gameObject.SetActive(false);
        // 可选：恢复游戏
        // Time.timeScale = 1;
    }
}