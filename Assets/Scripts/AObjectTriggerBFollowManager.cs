using UnityEngine;
using UnityEngine.UI;

public class AObjectTriggerBFollowManager : MonoBehaviour
{
    [Header("核心物体配置")]
    public GameObject objectA; // 拖拽赋值：A物体
    public GameObject objectB; // 拖拽赋值：B物体
    public GameObject panelUI; // 拖拽赋值：需要触发的根Panel
    public GameObject residualPanel; // 可选赋值：视觉残留的Panel（不赋值也不会报错）

    [Header("提示图片配置（A物体的子物体）")]
    public GameObject tipImage; // 拖拽赋值：A物体上隐藏的图片（TipImage）

    [Header("检测与跟随配置")]
    public float detectDistance = 3f; // A物体靠近区域的触发距离（可调整）
    private bool isPanelActive = false; // Panel是否激活标记
    private bool isBFollowing = false; // B物体是否已开启跟随标记
    private bool isInDetectRange = false; // A物体是否在检测范围内
    private CanvasGroup panelCanvasGroup; // 缓存目标Panel的CanvasGroup组件
    private CanvasGroup residualCanvasGroup; // 缓存残留Panel的CanvasGroup组件

    void Start()
    {
        // 初始化目标Panel
        if (panelUI != null)
        {
            panelCanvasGroup = panelUI.GetComponent<CanvasGroup>();
            panelUI.SetActive(false);
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = 0;
                panelCanvasGroup.interactable = false;
                panelCanvasGroup.blocksRaycasts = false;
            }
        }
        else
        {
            Debug.LogError("请为panelUI字段赋值目标根Panel对象！");
        }

        // 初始化残留Panel（关键：增加非空判断，未赋值时不执行任何操作，避免报错）
        if (residualPanel != null)
        {
            residualCanvasGroup = residualPanel.GetComponent<CanvasGroup>();
            residualPanel.SetActive(false);
            if (residualCanvasGroup != null)
            {
                residualCanvasGroup.alpha = 0;
                residualCanvasGroup.interactable = false;
                residualCanvasGroup.blocksRaycasts = false;
            }
        }
        // 移除强制警告，改为可选提示，未赋值时不报错
        else
        {
            Debug.Log("未赋值residualPanel，将跳过残留Panel的控制逻辑（正常不影响功能）");
        }

        // 初始化提示图片
        if (tipImage != null)
        {
            tipImage.SetActive(false);
        }
        else
        {
            Debug.LogWarning("请为tipImage字段赋值提示图片对象！");
        }

        isPanelActive = false;
    }

    void Update()
    {
        // 1. 检测A物体是否靠近触发区域
        CheckADistanceToArea();

        // 2. 若A在范围内，触发所有需要显示的Panel
        if (isInDetectRange && !isPanelActive && panelUI != null)
        {
            TriggerAllPanelsAndShowImage();
        }

        // 3. F键退出Panel（同步隐藏目标Panel + 可选残留Panel）
        if (Input.GetKeyDown(KeyCode.F) && isPanelActive && panelUI != null)
        {
            ExitAllPanelsAndHideImage();
        }

        // 4. B物体自动跟随A物体
        if (isBFollowing && objectA != null && objectB != null)
        {
            FollowObjectA();
        }
    }

    /// <summary>
    /// 检测A物体与触发区域的距离
    /// </summary>
    private void CheckADistanceToArea()
    {
        if (objectA == null) return;

        // 3D场景用Vector3.Distance，2D场景替换为Vector2.Distance即可
        float distance = Vector3.Distance(objectA.transform.position, this.transform.position);
        isInDetectRange = distance <= detectDistance;
    }

    /// <summary>
    /// 同步显示目标Panel + 可选残留Panel + 提示图片
    /// </summary>
    private void TriggerAllPanelsAndShowImage()
    {
        // 显示目标Panel
        panelUI.SetActive(true);
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 1;
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
        }

        // 关键：增加非空判断，residualPanel未赋值时不执行，避免报错
        if (residualPanel != null)
        {
            residualPanel.SetActive(true);
            if (residualCanvasGroup != null)
            {
                residualCanvasGroup.alpha = 1;
                residualCanvasGroup.interactable = true;
                residualCanvasGroup.blocksRaycasts = true;
            }
        }

        isPanelActive = true;

        // 显示提示图片
        if (tipImage != null)
        {
            tipImage.SetActive(true);
        }

        // 若需要显示Panel时暂停游戏，保留这行；不需要则删除
        // Time.timeScale = 0;
    }

    /// <summary>
    /// F键退出：同步隐藏目标Panel + 可选残留Panel + 提示图片（彻底消除报错）
    /// </summary>
    private void ExitAllPanelsAndHideImage()
    {
        // 日志打印
        Debug.Log($"是否执行隐藏Panel：panelUI是否为null={panelUI == null}，Panel当前Active状态={panelUI.activeSelf}");

        // 恢复游戏时间缩放
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 1. 隐藏目标Panel
        panelUI.SetActive(false);
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }

        // 关键：增加非空判断，residualPanel未赋值时不执行，彻底规避空引用报错
        if (residualPanel != null)
        {
            residualPanel.SetActive(false);
            if (residualCanvasGroup != null)
            {
                residualCanvasGroup.alpha = 0;
                residualCanvasGroup.interactable = false;
                residualCanvasGroup.blocksRaycasts = false;
            }
            Debug.Log($"残留Panel已隐藏，当前Active状态={residualPanel.activeSelf}");
        }

        // 3. 隐藏提示图片
        if (tipImage != null)
        {
            tipImage.SetActive(false);
        }

        // 4. 开启B物体跟随
        isPanelActive = false;
        isBFollowing = true;

        // 验证隐藏状态（增加空值判断，避免residualPanel未赋值时报错）
        float targetAlpha = panelCanvasGroup != null ? panelCanvasGroup.alpha : 0;
        float residualAlpha = (residualCanvasGroup != null && residualPanel != null) ? residualCanvasGroup.alpha : 0;
        bool residualActive = residualPanel != null ? residualPanel.activeSelf : false;
        Debug.Log($"隐藏后：目标Panel Active={panelUI.activeSelf}（Alpha={targetAlpha}），残留Panel Active={residualActive}（Alpha={residualAlpha}）");
    }

    /// <summary>
    /// B物体自动跟随A物体（平滑跟随，可调整速度）
    /// </summary>
    private void FollowObjectA()
    {
        float followSpeed = 5f; // 跟随速度，数值越大跟随越快
        Vector3 targetPos = objectA.transform.position;

        // 平滑插值更新位置
        objectB.transform.position = Vector3.Lerp(
            objectB.transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );

        // 可选：3D场景让B物体朝向A物体
        // objectB.transform.LookAt(objectA.transform);

        // 可选：2D场景让B物体朝向A物体
        // Vector2 direction = (Vector2)objectA.transform.position - (Vector2)objectB.transform.position;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // objectB.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}