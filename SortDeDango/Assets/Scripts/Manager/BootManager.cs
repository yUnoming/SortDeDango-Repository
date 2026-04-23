using UnityEngine;

public class BootManager : SceneManagerBase<BootManager>
{
    [SerializeField, Tooltip("起動シーン以外で開始するかどうか")]
    private bool isDebugStart = false;
    [SerializeField, Tooltip("開始シーンの種類")]
    private SceneType startSceneType = SceneType.Title;

    protected override void StateRunning()
    {
        Initialize();
        if(isDebugStart) ChangeScene(startSceneType, false);
        else ChangeScene(startSceneType, true);
    }
    
    private void Initialize()
    {
        SaveDataManager.Instance.Load();
    }
}
