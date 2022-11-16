using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonManager : MonoBehaviour
{
    [SerializeField]
    Button _tutolial = default;

    [SerializeField]
    Button _solo = default;

    [SerializeField]
    Button _local = default;

    [SerializeField]
    Button _online = default;

    private void Start()
    {
        System.Action<GameMode> action = (x) =>
        {
            LoadSceneManager.Instance.LoadScene("GameScene");
            SettingGameMode(x);
        };

        _tutolial?.onClick.AddListener(() => action.Invoke(GameMode.Tutorial));
        _solo?.onClick.AddListener(() => action.Invoke(GameMode.Solo));
        _local?.onClick.AddListener(() => action.Invoke(GameMode.Local));
        _online?.onClick.AddListener(() => action.Invoke(GameMode.Online));
    }

    private void SettingGameMode(GameMode gameMode)
    {
        AppManager.Instance.GameMode = gameMode;
    }
}
