using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField, Tooltip("遷移シーンタイプ")]
    private SceneType sceneType;
    [SerializeField, Tooltip("遷移シーン名")]
    private string sceneName;
    [SerializeField, Tooltip("シーンを再ロードするかどうか")]
    private bool isReload;

    private SceneManagerBase sceneManager;

    void Awake()
    {
        sceneManager = SceneManagerBase.activeManager;
        GetComponent<Button>().onClick.AddListener(ChangeScene);
    }
    private void ChangeScene()
    {
        if(isReload)
        {
            if(SceneManagerBase.sceneName != null) sceneManager.ChangeScene(sceneType, true, SceneManagerBase.sceneName);
            else sceneManager.ChangeScene(sceneType, true, SceneManager.GetActiveScene().name);
        }
        else sceneManager.ChangeScene(sceneType, true, sceneName);
    }
}
