using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DecibelMeter : MonoBehaviour
{
    [Header("UI组件绑定")]
    public Slider decibelSlider;
    public Text decibelText; // 旧版Text（可选）
    public TextMeshProUGUI decibelTMPText; // TMP Text（推荐，可选）
    public Image decibelFillImage; // Fill的Image组件

    [Header("全屏UI配置（新增）")]
    public GameObject fullScreenUI; // 拖入你的全屏UI面板
    public Button closeUIButton; // 拖入UI上的关闭按钮
    public float fadeInDuration = 0.8f; // 渐变显现时间（默认0.8秒）

    [Header("分贝数值配置")]
    public float increaseAmount = 20f;
    public float decreaseSpeed = 0.5f;
    public float maxDecibelValue = 100f;

    private float currentDecibelValue = 0f;
    private float currentFadeAlpha = 0f; // 渐变透明度
    private Image fullScreenUIImage; // 全屏UI的图片组件（用于渐变）
    private bool isUIShowing = false; // UI是否正在显示
    private bool isFadingIn = false; // 是否正在渐变显现
    private bool hasTriggeredUI = false; // 标记是否已触发过UI（避免重复触发）

    void Start()
    {
        // 初始化全屏UI
        InitFullScreenUI();
    }

    void Update()
    {
        // 数值衰减逻辑（保留原有功能）
        currentDecibelValue = Mathf.Max(currentDecibelValue - decreaseSpeed * Time.deltaTime, 0f);
        float normalizedValue = Mathf.InverseLerp(0f, maxDecibelValue, currentDecibelValue);

        // 更新Slider数值（保留原有功能）
        if (decibelSlider != null)
        {
            decibelSlider.value = normalizedValue;
        }

        // 计算渐变颜色并同步更新Fill和Text（保留原有功能）
        Color gradientColor = GetGradientColor(normalizedValue);
        UpdateFillColor(gradientColor);
        UpdateTextColorAndValue(gradientColor);

        // 新增：检测数值是否达到100，触发全屏UI（修复不弹出问题）
        CheckDecibelMaxValue();

        // 新增：全屏UI渐变显现逻辑（增加空值判断，避免报错）
        UpdateUIFadeIn();

        // 重置触发标记：当数值低于99时，允许再次触发UI
        if (currentDecibelValue < 99f)
        {
            hasTriggeredUI = false;
        }
    }

    // 初始化全屏UI（新增：增加空值判断，修复引用错误）
    private void InitFullScreenUI()
    {
        // 先判断全屏UI是否为空，避免空引用
        if (fullScreenUI == null)
        {
            Debug.LogError("【DecibelMeter】请为fullScreenUI绑定全屏UI面板！");
            return;
        }

        // 获取全屏UI的Image组件（若没有则自动添加，增加空值判断）
        fullScreenUIImage = fullScreenUI.GetComponent<Image>();
        if (fullScreenUIImage == null)
        {
            fullScreenUIImage = fullScreenUI.AddComponent<Image>();
            // 默认设置黑色半透背景（可在Inspector修改）
            fullScreenUIImage.color = new Color(0, 0, 0, 0);
            fullScreenUIImage.raycastTarget = true; // 防止穿透点击
        }

        // 初始隐藏全屏UI
        fullScreenUI.SetActive(false);
        currentFadeAlpha = 0f;
        UpdateUIFadeAlpha();

        // 绑定按钮点击事件（增加空值判断，避免报错）
        if (closeUIButton != null)
        {
            closeUIButton.onClick.RemoveAllListeners();
            closeUIButton.onClick.AddListener(OnCloseUIButtonClicked);
        }
        else
        {
            Debug.LogWarning("【DecibelMeter】请为closeUIButton绑定关闭按钮！");
        }
    }

    // 检测分贝是否达到最大值100（修复不弹出问题，增加多重判断）
    private void CheckDecibelMaxValue()
    {
        // 条件优化：数值达到100 + UI未显示 + 未在渐变中 + UI不为空 + 未触发过
        if (currentDecibelValue >= maxDecibelValue - 0.1f // 容错判断，避免浮点精度问题
            && !isUIShowing
            && !isFadingIn
            && fullScreenUI != null
            && !hasTriggeredUI)
        {
            TriggerFullScreenUI();
            hasTriggeredUI = true; // 标记已触发，避免重复调用
        }
    }

    // 触发全屏UI渐变显现（新增：隐藏分贝Slider和Text）
    private void TriggerFullScreenUI()
    {
        if (fullScreenUI == null) return; // 空值保护
        fullScreenUI.SetActive(true);
        isFadingIn = true;
        currentFadeAlpha = 0f;
        UpdateUIFadeAlpha();

        // ========== 新增：隐藏Canvas中的分贝Slider和Text ==========
        HideDecibelUIElements();
    }

    // 更新UI渐变显现效果（修复引用错误，增加空值判断）
    private void UpdateUIFadeIn()
    {
        // 多重空值判断，避免访问已销毁/为空的对象
        if (!isFadingIn || fullScreenUIImage == null || fullScreenUI == null)
        {
            return;
        }

        // 渐变增加透明度
        currentFadeAlpha += Time.deltaTime / fadeInDuration;
        currentFadeAlpha = Mathf.Clamp01(currentFadeAlpha); // 限制在0~1之间
        UpdateUIFadeAlpha();

        // 渐变完成
        if (currentFadeAlpha >= 1f)
        {
            isFadingIn = false;
            isUIShowing = true;
        }
    }

    // 更新UI透明度（增加空值判断）
    private void UpdateUIFadeAlpha()
    {
        if (fullScreenUIImage == null) return; // 空值保护
        Color uiColor = fullScreenUIImage.color;
        uiColor.a = currentFadeAlpha;
        fullScreenUIImage.color = uiColor;
    }

    // 按钮点击事件：关闭UI并清零数值（新增：恢复显示分贝Slider和Text）
    private void OnCloseUIButtonClicked()
    {
        // 空值判断，避免访问已销毁的对象
        if (fullScreenUI == null || fullScreenUIImage == null)
        {
            return;
        }

        // 隐藏UI
        fullScreenUI.SetActive(false);
        isUIShowing = false;
        isFadingIn = false;
        currentFadeAlpha = 0f;
        UpdateUIFadeAlpha();

        // 清零分贝数值
        currentDecibelValue = 0f;
        hasTriggeredUI = false; // 重置触发标记，允许下次再次触发
        Debug.Log("【DecibelMeter】全屏UI已关闭，分贝数值已清零！");

        // ========== 新增：恢复显示Canvas中的分贝Slider和Text ==========
        ShowDecibelUIElements();
    }

    // ========== 新增：隐藏分贝相关UI元素（Slider、Text、TMP Text） ==========
    private void HideDecibelUIElements()
    {
        // 隐藏Slider
        if (decibelSlider != null)
        {
            decibelSlider.gameObject.SetActive(false);
        }
        // 隐藏旧版Text
        if (decibelText != null)
        {
            decibelText.gameObject.SetActive(false);
        }
        // 隐藏TMP Text
        if (decibelTMPText != null)
        {
            decibelTMPText.gameObject.SetActive(false);
        }
        // 隐藏FillImage（可选，若需要同步隐藏）
        if (decibelFillImage != null)
        {
            decibelFillImage.gameObject.SetActive(false);
        }
    }

    // ========== 新增：显示分贝相关UI元素（Slider、Text、TMP Text） ==========
    private void ShowDecibelUIElements()
    {
        // 显示Slider
        if (decibelSlider != null)
        {
            decibelSlider.gameObject.SetActive(true);
        }
        // 显示旧版Text
        if (decibelText != null)
        {
            decibelText.gameObject.SetActive(true);
        }
        // 显示TMP Text
        if (decibelTMPText != null)
        {
            decibelTMPText.gameObject.SetActive(true);
        }
        // 显示FillImage（可选，对应隐藏逻辑）
        if (decibelFillImage != null)
        {
            decibelFillImage.gameObject.SetActive(true);
        }
    }

    // 触发器调用：增加分贝值（保留原有功能，增加数值限制）
    public void AddDecibels(float amount)
    {
        if (amount > 0)
        {
            currentDecibelValue += amount;
            currentDecibelValue = Mathf.Min(currentDecibelValue, maxDecibelValue);
        }
    }

    // 根据归一化数值计算绿→黄→红渐变颜色（保留原有功能）
    private Color GetGradientColor(float normalizedValue)
    {
        // 绿(0.33) → 黄(0.16) → 红(0)的HSV色相映射
        float hue = Mathf.Lerp(0.33f, 0f, normalizedValue);
        return Color.HSVToRGB(hue, 1f, 1f);
    }

    // 更新Fill的颜色（增加空值判断）
    private void UpdateFillColor(Color targetColor)
    {
        if (decibelFillImage == null) return; // 空值保护
        decibelFillImage.color = targetColor;
    }

    // 同步更新Text的数值和颜色（增加空值判断）
    private void UpdateTextColorAndValue(Color targetColor)
    {
        int intDecibel = Mathf.RoundToInt(currentDecibelValue);

        // 更新TMP Text（优先，增加空值判断）
        if (decibelTMPText != null)
        {
            decibelTMPText.text = intDecibel.ToString();
            decibelTMPText.color = targetColor;
        }

        // 更新旧版Text（增加空值判断）
        if (decibelText != null)
        {
            decibelText.text = intDecibel.ToString();
            decibelText.color = targetColor;
        }
    }

    // 额外保护：在脚本禁用/销毁时，重置状态（避免残留引用）
    void OnDisable()
    {
        isUIShowing = false;
        isFadingIn = false;
        hasTriggeredUI = false;
        currentFadeAlpha = 0f;
        // 脚本禁用时恢复显示分贝UI
        ShowDecibelUIElements();
    }

    void OnDestroy()
    {
        // 解除按钮绑定，避免内存泄漏
        if (closeUIButton != null)
        {
            closeUIButton.onClick.RemoveAllListeners();
        }
    }
}