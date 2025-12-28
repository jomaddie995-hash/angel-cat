using UnityEngine;

public class CharacterFlip : MonoBehaviour
{
    // 1. 在面板里把你的 Graphics 子物体拖到这个槽位
    public Transform graphicsTransform;
    private bool isFacingRight = true;

    void Update()
    {
        // 使用 GetAxisRaw 可以获得更干脆的输入（0, 1, 或 -1）
        float moveX = Input.GetAxisRaw("Horizontal");

        if (moveX > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveX < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        // 2. 关键修改：操作的是子物体的 Scale
        Vector3 theScale = graphicsTransform.localScale;
        theScale.x *= -1;
        graphicsTransform.localScale = theScale;
    }
}