using UnityEngine;

public class GameplayManager : SceneManagerBase<GameplayManager>
{
    [SerializeField, Tooltip("クリア目標となる食べた団子数")]
    private int targetDangoCount;
    [SerializeField, Tooltip("現在の食べた団子数")]
    private int eatenDangoCount;
    [SerializeField]
    private GameMode gameMode = GameMode.Normal;

    [Tooltip("リザルトUI")]
    private ResultUIController resultUI;

    [Tooltip("現在のゲームモード")]
    public GameMode CurrentGameMode => gameMode;

    protected override void StateInit()
    {
        // ステージのセットアップ
        StageData data = StageManager.Instance.CurrentStageData;
        FindAnyObjectByType<StageGenerator>().Generate(data);   // ステージ生成
        targetDangoCount = data.targetDangoCount;               // 目標数のセット
        // リザルトUIの初期設定
        resultUI = FindAnyObjectByType<ResultUIController>();
        resultUI.onNextClicked += HandleNextClicked;
        resultUI.Hide();

        base.StateInit();
    }
    protected override void StateStart()
    {
        // 値の初期化
        eatenDangoCount = 0;
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
/*                else if (IsGameOver())
                {
                    Debug.Log("ゲームオーバー！！");
                    PauseGame();
                }*/
                break;
        }
    }

    /// <summary>
    /// クリア状態か判定    </summary>
    /// <returns>
    /// 串を必要な数だけ完成していたかどうか </returns>
    private bool IsClear()
    {
        return eatenDangoCount >= targetDangoCount;
    }
    /// <summary>
    /// ゲームオーバー状態か判定    </summary>
    /// <returns>
    /// 串の現存数が許容数を超えているかどうか    </returns>
    private bool IsGameOver()
    {
        return false;
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
    /// 団子を食べた際のイベント    </summary>
    /// <param name="eatenCount">
    /// 食べた個数    </param>
    public void OnDangoEaten(int eatenCount)
    {
        eatenDangoCount += eatenCount;
    }
}
