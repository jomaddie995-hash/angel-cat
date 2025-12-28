using UnityEngine;
using UnityEngine.UI;

public class CatInteraction : MonoBehaviour
{
    public GameObject interactHint; // 拖入“按F键交互”的UI
    public GameObject dialogueBox;  // 拖入“对话框”的UI
    private bool isPlayerNearby = false;

    void Update()
    {
        // 如果玩家在附近，并且按下了 F 键
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            ToggleDialogue();
        }
    }

    void ToggleDialogue()
    {

        // 逻辑：按下F后，隐藏提示，显示对话框
        interactHint.SetActive(false);
        dialogueBox.SetActive(!dialogueBox.activeSelf);
    }

    // 进入感应区
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player")) // 记得给小猫设置 Tag 为 Player
        {
            Debug.Log(other.gameObject);
            isPlayerNearby = true;
            interactHint.SetActive(true);
        }
    }

    // 离开感应区
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactHint.SetActive(false);
            dialogueBox.SetActive(false); // 走远了自动关闭对话框
        }
    }
}
