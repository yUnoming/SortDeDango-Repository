using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField, Tooltip("選択ステージ番号")]
    private int stageIndex;

    private SceneManagerBase sceneManager;

    void Awake()
    {
        sceneManager = SceneManagerBase.activeManager;
        GetComponent<Button>().onClick.AddListener(SelectStage);
    }
    private void SelectStage()
    {
        StageManager.Instance.SetStage(stageIndex);
        sceneManager.ChangeScene(SceneType.Gameplay);
    }
}
