using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayManager : SceneManagerBase<GameplayManager>
{
    private StageGenerator stageGenerator;
    private List<SkewerController> skewerList = new List<SkewerController>();
    [Tooltip("クリアしたかどうか")]
    private bool isClear;
    [Tooltip("リザルトUI")]
    private ResultUIController resultUI;

    protected override void StateInit()
    {
        // ステージ生成
        stageGenerator = FindAnyObjectByType<StageGenerator>();
        skewerList = stageGenerator.Generate(StageManager.Instance.CurrentStageData);
        // リザルトUIの初期設定
        resultUI = FindAnyObjectByType<ResultUIController>();
        resultUI.OnNextClicked += HandleNextClicked;
        resultUI.Hide();

        base.StateInit();
    }
    protected override void StateStart()
    {
        isClear = false;    // クリア状態の初期化
        base.StateStart();
    }
    protected override void StateRunning()
    {
        // クリア判定
        if (IsClear() && !isClear)
        {
            // クリア表示
            Debug.Log("ステージクリア！！");
            resultUI.Show();

            PauseGame();
            isClear = true;
        }
    }

    /// <summary>
    /// クリア状態か判定    </summary>
    /// <returns>
    /// 全串が空or完成状態: TRUE / ひとつでも条件を満たさない: FALSE    </returns>
    private bool IsClear()
    {
        // 全串の状態判定
        foreach (var skewer in skewerList)
        {
            // 空or完成状態でなければ、FALSEを返す
            if (skewer.CurrentState != SkewerState.Empty && skewer.CurrentState != SkewerState.Complete)
                return false;
        }
        // ここまで来たら条件達成のため、TRUEを返す
        return true;
    }
    /// <summary>
    /// Nextボタン押下時の処理    </summary>
    private void HandleNextClicked()
    {
        LoadNextStage();
    }

    /// <summary>
    /// ゲームを停止    </summary>
    public void PauseGame()
    {
        // 串の機能停止
        foreach(var skewer in skewerList) skewer.enabled = false;
    }
    /// <summary>
    /// ゲームを再開   </summary>
    public void ResumeGame()
    {
        // 串の機能再開
        foreach (var skewer in skewerList) skewer.enabled = true;
    }

    /// <summary>
    /// ステージを初期化    </summary>
    public void ResetStage()
    {
        ChangeScene(SceneType.Gameplay, true, sceneName);
    }
    /// <summary>
    /// 次のステージをロード    </summary>
    public void LoadNextStage()
    {
        StageManager.Instance.GoToNextStage();
        ResetStage();
    }
    /// <summary>
    /// 前のステージをロード    </summary>
    public void LoadPreviousStage()
    {
        StageManager.Instance.GoToPreviousStage();
        ResetStage();
    }
}
