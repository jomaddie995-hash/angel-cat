using UnityEngine;
using UnityEngine.UI;

public class CatInteraction : MonoBehaviour
{
    // 拖入你想要显示的悬浮图片（必须是Sprite格式）
    public Sprite interactSprite;
    // 拖入“对话框”的UI（保留原有功能）
    public GameObject dialogueBox;

    // 图片渲染层级（可根据你的项目调整）
    public string sortingLayerName = "UI";
    // 图片层级顺序（数值越高越在上方）
    public int sortingOrder = 10;

    // 已按你的要求固定Transform参数
    private readonly Vector3 spriteLocalPosition = new Vector3(-0.09f, -0.03f, -1.53f);
    private readonly Vector3 spriteLocalRotation = new Vector3(-175.1f, 89.985f, -89.82f);
    private readonly Vector3 spriteLocalScale = new Vector3(1.4192f, 1.4448f, 1f);

    private bool isPlayerNearby = false;
    private GameObject interactSpriteInstance; // 悬浮图片实例
    private Vector3 originalScale; // 物体原始缩放（备用）

    void Start()
    {
        // 1. 检查是否分配了悬浮图片，避免报错
        if (interactSprite == null)
        {
            Debug.LogError("【CatInteraction】请在Inspector面板为interactSprite分配一张Sprite格式的图片！");
            enabled = false; // 禁用脚本，防止后续错误
            return;
        }

        // 2. 创建悬浮图片对象
        interactSpriteInstance = new GameObject("InteractFloatingSprite");
        // 将图片设为当前物体的子物体，跟随物体移动
        interactSpriteInstance.transform.SetParent(transform, false);

        // 应用你指定的Transform参数（核心：直接赋值固定值）
        interactSpriteInstance.transform.localPosition = spriteLocalPosition;
        interactSpriteInstance.transform.localEulerAngles = spriteLocalRotation;
        interactSpriteInstance.transform.localScale = spriteLocalScale;

        // 3. 添加SpriteRenderer组件（场景中显示图片的核心）
        SpriteRenderer spriteRenderer = interactSpriteInstance.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = interactSprite;
        spriteRenderer.sortingLayerName = sortingLayerName; // 设置渲染层级，确保能看到
        spriteRenderer.sortingOrder = sortingOrder; // 设置层级优先级，避免被其他物体遮挡

        // 4. 记录物体原始缩放（备用，可删除不影响核心功能）
        originalScale = transform.localScale;

        // 5. 默认隐藏悬浮图片（玩家靠近后再显示）
        interactSpriteInstance.SetActive(false);

        // 调试信息：确认图片创建成功及Transform参数
        Debug.Log($"【CatInteraction】悬浮图片创建成功");
        Debug.Log($"位置：{interactSpriteInstance.transform.localPosition}");
        Debug.Log($"旋转：{interactSpriteInstance.transform.localEulerAngles}");
        Debug.Log($"缩放：{interactSpriteInstance.transform.localScale}");
    }

    void Update()
    {
        // 保留原有功能：玩家在附近且按下F键，触发交互
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            ToggleDialogue();
        }
    }

    void ToggleDialogue()
    {
        // 保留原有逻辑：按下F后隐藏悬浮图片，切换对话框显示状态
        if (interactSpriteInstance != null)
        {
            interactSpriteInstance.SetActive(false);
        }
        dialogueBox.SetActive(!dialogueBox.activeSelf);
    }

    // 进入感应区：显示悬浮图片
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"【CatInteraction】玩家靠近物体：{gameObject.name}");
            isPlayerNearby = true;
            // 显示悬浮图片
            if (interactSpriteInstance != null)
            {
                interactSpriteInstance.SetActive(true);
            }
        }
    }

    // 离开感应区：隐藏悬浮图片和对话框
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            // 隐藏悬浮图片
            if (interactSpriteInstance != null)
            {
                interactSpriteInstance.SetActive(false);
            }
            // 自动关闭对话框
            dialogueBox.SetActive(false);
            // 重置物体缩放（备用功能，可删除）
            transform.localScale = originalScale;

            Debug.Log($"【CatInteraction】玩家离开物体：{gameObject.name}");
        }
    }
}