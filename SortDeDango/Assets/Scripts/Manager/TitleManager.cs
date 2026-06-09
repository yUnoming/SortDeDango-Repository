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
        // 到達ステージ番号が`0(初期状態)`なら一部ボタンを非表示
        titleUI.UpdateButtonInteractableBySaveData(
            SaveDataManager.Instance.Get<GameplayData>().reachedStageIndex != 0 ?
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
        SaveDataManager.Instance.CreateNewSaveData();
        StageManager.Instance.SetStage(SaveDataManager.Instance.Get<GameplayData>().reachedStageIndex);
        ChangeScene(SceneType.Gameplay);
    }
    /// <summary>
    /// Continueボタン押下時の処理    </summary>
    private void HandleContinueClicked()
    {
        StageManager.Instance.SetStage(SaveDataManager.Instance.Get<GameplayData>().lastPlayedStageIndex);
        ChangeScene(SceneType.Gameplay);
    }
}
