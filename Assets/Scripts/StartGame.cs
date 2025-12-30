using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [Header("场景名字")]
    public string sceneName;

    [Header("全屏过渡UI")]
    public GameObject fullScreenUI; // 拖入你的全屏UI预制体/物体（需提前设置好全屏样式）

    private bool isShowingTransitionUI = false; // 标记是否已显示全屏UI

    void Start()
    {
        // 初始化：默认隐藏全屏UI
        if (fullScreenUI != null)
        {
            fullScreenUI.SetActive(false);
        }
        else
        {
            Debug.LogError("【StartGame】请在Inspector面板为fullScreenUI拖入全屏UI物体！");
        }
    }

    public void StartMenu()
    {
        // 防止重复触发
        if (isShowingTransitionUI || fullScreenUI == null)
        {
            return;
        }

        // 1. 显示全屏UI
        fullScreenUI.SetActive(true);
        isShowingTransitionUI = true;

        // 2. 启动协程，等待玩家输入（任意键/鼠标左键）
        StartCoroutine(WaitForPlayerInputThenLoadScene());
    }

    /// <summary>
    /// 协程：等待玩家输入后加载场景
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForPlayerInputThenLoadScene()
    {
        // 循环等待，直到检测到任意键按下 或 鼠标左键点击
        while (true)
        {
            // 检测任意键盘按键按下
            bool hasKeyInput = Input.anyKeyDown;
            // 检测鼠标左键点击（0对应左键）
            bool hasMouseLeftClick = Input.GetMouseButtonDown(0);

            // 若检测到任意一种输入，退出循环
            if (hasKeyInput || hasMouseLeftClick)
            {
                break;
            }

            // 等待下一帧，继续检测
            yield return null;
        }

        // 3. 检测到输入后，加载目标场景
        if (!string.IsNullOrEmpty(sceneName))
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
        else
        {
            Debug.LogError("【StartGame】请在Inspector面板设置sceneName（目标场景名称）！");
        }
    }
}