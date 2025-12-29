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

    [Header("分贝数值配置")]
    public float increaseAmount = 20f;
    public float decreaseSpeed = 0.5f;
    public float maxDecibelValue = 100f;

    private float currentDecibelValue = 0f;

    void Update()
    {
        // 数值衰减逻辑
        currentDecibelValue = Mathf.Max(currentDecibelValue - decreaseSpeed * Time.deltaTime, 0f);
        float normalizedValue = Mathf.InverseLerp(0f, maxDecibelValue, currentDecibelValue);

        // 更新Slider数值
        if (decibelSlider != null)
        {
            decibelSlider.value = normalizedValue;
        }

        // 计算渐变颜色并同步更新Fill和Text
        Color gradientColor = GetGradientColor(normalizedValue);
        UpdateFillColor(gradientColor);
        UpdateTextColorAndValue(gradientColor);
    }

    // 触发器调用：增加分贝值
    public void AddDecibels(float amount)
    {
        if (amount > 0)
        {
            currentDecibelValue += amount;
            currentDecibelValue = Mathf.Min(currentDecibelValue, maxDecibelValue);
        }
    }

    // 根据归一化数值计算绿→黄→红渐变颜色
    private Color GetGradientColor(float normalizedValue)
    {
        // 绿(0.33) → 黄(0.16) → 红(0)的HSV色相映射
        float hue = Mathf.Lerp(0.33f, 0f, normalizedValue);
        return Color.HSVToRGB(hue, 1f, 1f);
    }

    // 更新Fill的颜色
    private void UpdateFillColor(Color targetColor)
    {
        if (decibelFillImage != null)
        {
            decibelFillImage.color = targetColor;
        }
    }

    // 同步更新Text的数值和颜色
    private void UpdateTextColorAndValue(Color targetColor)
    {
        int intDecibel = Mathf.RoundToInt(currentDecibelValue);

        // 更新TMP Text（优先）
        if (decibelTMPText != null)
        {
            decibelTMPText.text = intDecibel.ToString();
            decibelTMPText.color = targetColor;
        }

        // 更新旧版Text
        if (decibelText != null)
        {
            decibelText.text = intDecibel.ToString();
            decibelText.color = targetColor;
        }
    }
}