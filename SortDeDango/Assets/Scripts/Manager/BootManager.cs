using UnityEngine;

public class BootManager : SceneManagerBase<BootManager>
{
    protected override void StateRunning()
    {
        SaveDataManager.Instance.Load();
        ChangeScene(SceneType.Title, true, "TitleScene");
    }
}
