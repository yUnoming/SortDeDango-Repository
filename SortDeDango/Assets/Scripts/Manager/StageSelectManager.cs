using UnityEngine;

public class StageSelectManager : SceneManagerBase<StageSelectManager>
{
    [SerializeField]
    private AudioData bgm;

    protected override void StateInit()
    {
        // 現在のセーブデータから値を取得し、ステージ選択ボタンのロックを解除
        StageSelectUIController stageSelectUI = FindAnyObjectByType<StageSelectUIController>();
        stageSelectUI.UpdateStageSelectButtons(
            SaveDataManager.Instance.Get<GameplayData>().reachedStageIndex);

        base.StateInit();
    }
    protected override void StateStart()
    {
        AudioManager.Instance.PlayBGM(bgm);
        base.StateStart();
    }
}
