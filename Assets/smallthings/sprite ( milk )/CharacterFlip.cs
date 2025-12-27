using UnityEngine;

public class CharacterFlip : MonoBehaviour
{
    private bool isFacingRight = true; // 初始状态是否朝右

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");

        // 如果向右移动且当前朝向左，则翻转
        if (moveX > 0 && !isFacingRight)
        {
            Flip();
        }
        // 如果向左移动且当前朝向右，则翻转
        else if (moveX < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // 切换布尔值状态
        isFacingRight = !isFacingRight;

        // 获取当前的 localScale
        Vector3 theScale = transform.localScale;

        // 将 X 轴乘以 -1 实现镜像
        theScale.x *= -1;

        // 重新赋值回物体的 scale
        transform.localScale = theScale;
    }
}