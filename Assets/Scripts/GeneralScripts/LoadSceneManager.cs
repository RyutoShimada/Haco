using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

#if UNITY_EDITOR
    /////////////// �e�X�g���m�F�p ////////////////////////////////
    private void Start()
    {
        SceneManager.sceneLoaded += Test;
    }

    public void Test(Scene nextScene, LoadSceneMode mode)
    {
        Debug.Log($"GameMode is [ {AppManager.Instance.GameMode} ].");
    }
#endif
}
