using UnityEngine;
using UnityEngine.UI;

public class DecibelMeter : MonoBehaviour
{
    public Slider decibelSlider;
    public float increaseAmount = 20f;   // 每次触发增加的数值
    public float decreaseSpeed = 0.5f;   // 每秒衰减的数值
    public float maxDecibelValue = 100f; // 数值上限

    private float currentDecibelValue = 0f; // 当前的分贝值

    void Update()
    {
        // 1. 衰减：当前值 = 当前值 - 衰减速度 * 时间
        currentDecibelValue -= decreaseSpeed * Time.deltaTime;

        // 2. 限制在0到最大值之间
        currentDecibelValue = Mathf.Clamp(currentDecibelValue, 0, maxDecibelValue);

        // 3. 将0-100的数值映射到Slider的0-1范围
        decibelSlider.value = currentDecibelValue / maxDecibelValue;
    }

    // 这个方法用于外部触发增加分贝值
    public void AddDecibels(float amount)
    {
        currentDecibelValue += amount;
        currentDecibelValue = Mathf.Clamp(currentDecibelValue, 0, maxDecibelValue);
    }
}