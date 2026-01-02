using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shade : MonoBehaviour
{
    private Transform player;
    public RectTransform uiElement; // 需要转换到的UI元素的RectTransform
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;   
    }

    // Update is called once per frame
    void Update()
    {
        // 获取鼠标的屏幕坐标

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(player.position+new Vector3(0,0.5f,0));

        // 将屏幕坐标转换为UI坐标
        Vector2 uiPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiElement.parent.GetComponent<RectTransform>(),
            screenPoint,
            null, // 摄像机参数
            out uiPoint);

        // 输出转换后的UI坐标


        // 如果需要设置UI元素的位置，可以这样做
        uiElement.anchoredPosition = uiPoint;
    }
}
