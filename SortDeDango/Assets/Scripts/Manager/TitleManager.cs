using UnityEngine;

public class TitleManager : SceneManagerBase<TitleManager>
{
    [SerializeField]
    private AudioData bgm;

    protected override void StateInit()
    {
        // 各種ボタンのイベント設定
        TitleUIController titleUI = FindAnyObjectByType<TitleUIController>();
        titleUI.onNewGameClicked += HandleNewGameClicked;
        titleUI.onContinueClicked += HandleContinueClicked;
        // セーブデータの有無で入力受付更新
        titleUI.UpdateButtonInteractableBySaveData(
            SaveDataManager.Instance.CurrentSaveData != null ?
            true :
            false);

        base.StateInit();
    }
    protected override void StateStart()
    {
        AudioManager.Instance.PlayBGM(bgm);
        base.StateStart();
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
