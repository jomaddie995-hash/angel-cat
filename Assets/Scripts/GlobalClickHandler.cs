using UnityEngine;

public class GlobalClickHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Update()
    {
        // 0 代表鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            if (audioSource != null && clickSound != null)
            {
                // 使用 PlayOneShot 确保声音可以叠加且不被中断
                audioSource.PlayOneShot(clickSound);
            }
        }
    }
}
