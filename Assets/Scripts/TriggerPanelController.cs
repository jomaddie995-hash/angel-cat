using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 触发区域控制脚本：检测Player进入，显示全屏Panel，任意键关闭Panel（旧输入系统兼容版，无额外依赖）
/// </summary>
public class TriggerPanelController : MonoBehaviour
{
    [Header("全屏Panel引用")]
    public GameObject fullScreenPanel; // 拖拽赋值：场景中的全屏Panel对象

    // 面板显示状态标记
    private bool isPanelShowing = false;

    /// <summary>
    /// 初始化：确保Panel初始为隐藏状态
    /// </summary>
    private void Start()
    {
        if (fullScreenPanel != null)
        {
            fullScreenPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("请为TriggerPanelController赋值全屏Panel对象！");
        }
    }

    /// <summary>
    /// 触发器检测：当对象进入触发区域时调用
    /// </summary>
    /// <param name="other">进入触发区域的碰撞体</param>
    private void OnTriggerEnter(Collider other)
    {
        // 判断进入的对象是否是Player（性能更优的标签匹配方式）
        if (other.CompareTag("Player") && !isPanelShowing)
        {
            ShowFullScreenPanel(); // 显示全屏Panel
        }
    }

    /// <summary>
    /// 每一帧检测输入：用于关闭Panel（旧输入系统通用写法，无兼容性问题）
    /// </summary>
    private void Update()
    {
        // 只有当面板显示时，才检测输入
        if (isPanelShowing)
        {
            // 等效任意键检测：检测所有键盘按键 或 鼠标按键的按下瞬间
            bool anyKeyPressed = CheckAnyKeyOrMouseDown();
            if (anyKeyPressed)
            {
                HideFullScreenPanel(); // 隐藏全屏Panel
            }
        }
    }

    /// <summary>
    /// 检测任意键盘按键或鼠标按键按下（旧输入系统通用API，无报错）
    /// </summary>
    /// <returns>是否有任意键/鼠标按下</returns>
    private bool CheckAnyKeyOrMouseDown()
    {
        // 检测所有键盘按键（KeyCode枚举遍历，覆盖所有键盘键）
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }

        // 单独检测鼠标按键（也可通过KeyCode.Mouse0/1/2实现，此处更直观）
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 显示全屏Panel的方法
    /// </summary>
    private void ShowFullScreenPanel()
    {
        if (fullScreenPanel != null)
        {
            fullScreenPanel.SetActive(true);
            isPanelShowing = true;
            // 可选：显示面板时暂停游戏
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// 隐藏全屏Panel的方法
    /// </summary>
    private void HideFullScreenPanel()
    {
        if (fullScreenPanel != null)
        {
            fullScreenPanel.SetActive(false);
            isPanelShowing = false;
            // 可选：隐藏面板时恢复游戏
            Time.timeScale = 1;
        }
    }

    // 2D场景适配：取消注释即可使用
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player") && !isPanelShowing)
    //     {
    //         ShowFullScreenPanel();
    //     }
    // }
}