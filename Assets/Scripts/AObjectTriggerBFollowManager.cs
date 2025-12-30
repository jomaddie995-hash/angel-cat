using UnityEngine;

public class AObjectTriggerBFollowManager : MonoBehaviour
{
    [Header("核心物体配置")]
    public GameObject objectA; // 拖拽赋值：A物体
    public GameObject objectB; // 拖拽赋值：B物体
    public GameObject panelUI; // 拖拽赋值：需要触发的Panel

    [Header("提示图片配置（A物体的子物体）")]
    public GameObject tipImage; // 拖拽赋值：A物体上隐藏的图片（TipImage）

    [Header("检测与跟随配置")]
    public float detectDistance = 3f; // A物体靠近区域的触发距离（可调整）
    private bool isPanelActive = false; // Panel是否激活标记
    private bool isBFollowing = false; // B物体是否已开启跟随标记
    private bool isInDetectRange = false; // A物体是否在检测范围内

    void Start()
    {
        // 初始化状态：Panel隐藏 + 图片隐藏
        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }
        if (tipImage != null)
        {
            tipImage.SetActive(false); // 确保图片初始为隐藏状态
        }
    }

    void Update()
    {
        // 1. 检测A物体是否靠近触发区域
        CheckADistanceToArea();

        // 2. 若A在范围内，触发Panel和显示A物体上的图片
        if (isInDetectRange && !isPanelActive)
        {
            TriggerPanelAndShowImage();
        }

        // 3. F键退出Panel（仅当Panel激活时生效）
        if (Input.GetKeyDown(KeyCode.F) && isPanelActive)
        {
            ExitPanelAndHideImage();
        }

        // 4. B物体自动跟随A物体（仅当跟随标记开启时）
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

        // 判断是否在检测范围内
        isInDetectRange = distance <= detectDistance;
    }

    /// <summary>
    /// 触发Panel显示 + 显示A物体上原本隐藏的图片
    /// </summary>
    private void TriggerPanelAndShowImage()
    {
        // 激活Panel
        if (panelUI != null)
        {
            panelUI.SetActive(true);
            isPanelActive = true;
        }

        // 显示A物体上的提示图片
        if (tipImage != null)
        {
            tipImage.SetActive(true);
        }
    }

    /// <summary>
    /// F键退出Panel + 隐藏A物体上的图片 + 开启B物体跟随
    /// </summary>
    private void ExitPanelAndHideImage()
    {
        // 关闭Panel
        if (panelUI != null)
        {
            panelUI.SetActive(false);
            isPanelActive = false;
        }

        // 隐藏A物体上的提示图片
        if (tipImage != null)
        {
            tipImage.SetActive(false);
        }

        // 开启B物体跟随标记（一旦开启，持续跟随）
        isBFollowing = true;
    }

    /// <summary>
    /// B物体自动跟随A物体（平滑跟随，可调整速度）
    /// </summary>
    private void FollowObjectA()
    {
        // 方式1：瞬间跟随（直接赋值位置，无过渡效果）
        // objectB.transform.position = objectA.transform.position;

        // 方式2：平滑跟随（推荐，更自然，可调整followSpeed控制速度）
        float followSpeed = 5f; // 跟随速度，数值越大跟随越快
        Vector3 targetPos = objectA.transform.position; // B的目标位置（与A一致）

        // 平滑插值更新B物体位置
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