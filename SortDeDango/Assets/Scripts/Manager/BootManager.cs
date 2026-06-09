using UnityEngine;

public class BootManager : SceneManagerBase<BootManager>
{
    [SerializeField, Tooltip("起動シーン以外で開始するかどうか")]
    private bool isDebugStart = false;
    [SerializeField, Tooltip("開始シーンの種類")]
    private SceneType startSceneType = SceneType.Title;

    protected override void StateInit()
    {
        SaveDataManager.Instance.LoadAll();
        base.StateInit();
    }
    protected override void StateStart()
    {
        // デバッグ時は、現在のシーンから開始可能
        if (isDebugStart) ChangeScene(startSceneType, true);
        else ChangeScene(startSceneType, true);

        base.StateStart();
    }
}
