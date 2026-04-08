using UnityEngine;

public class BootManager : SceneManagerBase<BootManager>
{
    protected override void StateRunning()
    {
        // 起動時のセットアップ等はここで行う
        ChangeScene(SceneType.Gameplay, true, "GameplayScene");
    }
}
