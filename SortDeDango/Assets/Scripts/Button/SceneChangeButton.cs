using UnityEngine;
using UnityEngine.UI;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField, Tooltip("遷移シーンタイプ")]
    private SceneType sceneType;
    [SerializeField, Tooltip("シーンを遷移するかどうか")]
    private bool isLoadScene = true;
    [Header("デフォルトシーン以外に遷移したい場合に設定")]
    [SerializeField, Tooltip("上書き用遷移シーン名")]
    private string overrideSceneName;

    private SceneManagerBase sceneManager;

    void Awake()
    {
        sceneManager = SceneManagerBase.activeManager;
        GetComponent<Button>().onClick.AddListener(ChangeScene);
    }
    private void ChangeScene()
    {
        if (string.IsNullOrEmpty(overrideSceneName)) sceneManager.ChangeScene(sceneType, isLoadScene);
        else sceneManager.ChangeScene(sceneType, isLoadScene, overrideSceneName);
    }
}