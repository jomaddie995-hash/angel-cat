using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private bool canInteract = false;
    private NPCFollow followScript;

    void Start()
    {
        followScript = GetComponent<NPCFollow>();
    }

    void Update()
    {
        // 当玩家在范围内并按下 E 键
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            followScript.StartFollowing(); // 开启跟随
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canInteract = false;
    }
}