public class StageSelectManager : SceneManagerBase<StageSelectManager>
{
    protected override void StateInit()
    {
        // 現在のセーブデータから値を取得し、ステージ選択ボタンのロックを解除
        StageSelectUIController stageSelectUI = FindAnyObjectByType<StageSelectUIController>();
        stageSelectUI.UpdateStageSelectButtonsLock(SaveDataManager.Instance.CurrentSaveData.reachedStageIndex);
        base.StateInit();
    }
}
