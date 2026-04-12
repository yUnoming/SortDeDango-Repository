using UnityEngine;

public class TitleManager : SceneManagerBase<TitleManager>
{
    protected override void StateInit()
    {
        TitleUIController titleUI = FindAnyObjectByType<TitleUIController>();
        titleUI.onNewGameClicked += HandleNewGameClicked;
        base.StateInit();
    }

    /// <summary>
    /// NewGameボタン押下時の処理    </summary>
    private void HandleNewGameClicked()
    {
        SaveData newSaveData = SaveDataManager.Instance.CreateSaveData();
        StageManager.Instance.SetStage(newSaveData.reachedStageIndex);
        ChangeScene(SceneType.Gameplay, true, "GameplayScene");
    }
}
