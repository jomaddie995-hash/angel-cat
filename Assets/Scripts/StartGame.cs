using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [Header("场景名字")]
    public string sceneName;
    public void StartMenu()
    {
        SceneManager.LoadSceneAsync(sceneName);   //加载场景
    }
}
 