using UnityEngine;
using UnityEngine.UI;

public class CatInteraction : MonoBehaviour
{
    public GameObject interactSpriteObj; // 手动摆好位置的悬浮图片物体
    public GameObject dialogueBox;

    private bool isPlayerNearby = false;
    private Vector3 originalScale;

    void Start()
    {
        // 空值检查+日志提示
        if (interactSpriteObj == null)
        {
            Debug.LogError($"【CatInteraction】物体{gameObject.name}：未绑定interactSpriteObj！");
            enabled = false;
            return;
        }

        originalScale = transform.localScale;
        interactSpriteObj.SetActive(false);
        Debug.Log($"【CatInteraction】物体{gameObject.name}：初始化完成，悬浮图片已隐藏");

        // 额外检查：当前物体是否有触发碰撞体
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogWarning($"【CatInteraction】物体{gameObject.name}：缺少碰撞体或未勾选Is Trigger！");
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            ToggleDialogue();
        }
    }

    void ToggleDialogue()
    {
        if (interactSpriteObj != null)
        {
            interactSpriteObj.SetActive(false);
        }
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(!dialogueBox.activeSelf);
            Debug.Log($"【CatInteraction】物体{gameObject.name}：对话框状态切换为{dialogueBox.activeSelf}");
        }
    }

    // 触发进入：增加日志，便于排查是否检测到玩家
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"【CatInteraction】物体{gameObject.name}：检测到物体{other.gameObject.name}进入触发区域，Tag：{other.tag}");

        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactSpriteObj != null)
            {
                interactSpriteObj.SetActive(true);
                Debug.Log($"【CatInteraction】物体{gameObject.name}：玩家靠近，悬浮图片显示");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactSpriteObj != null)
            {
                interactSpriteObj.SetActive(false);
                Debug.Log($"【CatInteraction】物体{gameObject.name}：玩家离开，悬浮图片隐藏");
            }
            if (dialogueBox != null)
            {
                dialogueBox.SetActive(false);
            }
            transform.localScale = originalScale;
        }
    }
}