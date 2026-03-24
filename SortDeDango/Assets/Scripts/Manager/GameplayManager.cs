using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SceneManagerBase<GameplayManager>
{
    [SerializeField, Tooltip("クリアに必要な完成串数")]
    private int requiredCompleteSkewerCount = 3;
    [SerializeField, Tooltip("串の許容数")]
    private int maxSkewerCount = 4;
    [SerializeField]
    private GameMode gameMode = GameMode.Normal;

    private StageGenerator stageGenerator;
    private List<SkewerController> skewerList = new List<SkewerController>();
    [Tooltip("現在の完成串数")]
    private int currentCompleteSkewerCount;
    [Tooltip("リザルトUI")]
    private ResultUIController resultUI;

    [Tooltip("現在のゲームモード")]
    public GameMode CurrentGameMode => gameMode;

    protected override void StateInit()
    {
        // ステージ生成
        stageGenerator = FindAnyObjectByType<StageGenerator>();
        skewerList = stageGenerator.Generate(StageManager.Instance.CurrentStageData);
        // リザルトUIの初期設定
        resultUI = FindAnyObjectByType<ResultUIController>();
        resultUI.onNextClicked += HandleNextClicked;
        resultUI.Hide();

        base.StateInit();
    }
    protected override void StateStart()
    {
        // 値の初期化
        currentCompleteSkewerCount = 0;
        base.StateStart();
    }
    protected override void StateRunning()
    {
        // クリア判定
        switch(CurrentGameMode)
        {
            case GameMode.Normal:
                if (IsClear())
                {
                    // クリア表示
                    Debug.Log("ステージクリア！！");
                    resultUI.Show();

                    PauseGame();
                    base.StateRunning();
                }
                else if (IsGameOver())
                {
                    Debug.Log("ゲームオーバー！！");
                    PauseGame();
                }
                break;
        }
    }

    /// <summary>
    /// クリア状態か判定    </summary>
    /// <returns>
    /// 串を必要な数だけ完成していたかどうか </returns>
    private bool IsClear()
    {
        return currentCompleteSkewerCount >= requiredCompleteSkewerCount;
    }
    /// <summary>
    /// ゲームオーバー状態か判定    </summary>
    /// <returns>
    /// 串の現存数が許容数を超えているかどうか    </returns>
    private bool IsGameOver()
    {
        return skewerList.Count > maxSkewerCount;
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
        GameplayController.Instance.SetInputLocked(true);
    }
    /// <summary>
    /// ゲームを再開   </summary>
    public void ResumeGame()
    {
        GameplayController.Instance.SetInputLocked(false);
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

    /// <summary>
    /// 串を追加    </summary>
    public void AddSkewer(SkewerController skewer) { skewerList.Add(skewer); }
    // 串が完成した際のイベント
    public void OnSkewerCompleted(SkewerController skewer)
    {
        skewerList.Remove(skewer);      // 完成した串を除外
        currentCompleteSkewerCount++;   // 完成数を増加
    }
}
