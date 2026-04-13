using UnityEngine;

public class TitleManager : SceneManagerBase<TitleManager>
{
    protected override void StateInit()
    {
        // 各種ボタンのイベント設定
        TitleUIController titleUI = FindAnyObjectByType<TitleUIController>();
        titleUI.onNewGameClicked += HandleNewGameClicked;
        titleUI.onContinueClicked += HandleContinueClicked;
        // セーブデータがあれば、Continueボタンを入力可能に
        if (SaveDataManager.Instance.CurrentSaveData != null)
            titleUI.SetContinueButtonInteractable(true);

        base.StateInit();
    }

    /// <summary>
    /// NewGameボタン押下時の処理    </summary>
    private void HandleNewGameClicked()
    {
        SaveData newSaveData = SaveDataManager.Instance.CreateSaveData();
        StageManager.Instance.SetStage(newSaveData.reachedStageIndex);
        ChangeScene(SceneType.Gameplay);
    }
    /// <summary>
    /// Continueボタン押下時の処理    </summary>
    private void HandleContinueClicked()
    {
        StageManager.Instance.SetStage(SaveDataManager.Instance.Load().reachedStageIndex);
        ChangeScene(SceneType.Gameplay);
    }
}
