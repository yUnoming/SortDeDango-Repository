using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SceneManagerBase<GameplayManager>
{
    [SerializeField, Tooltip("クリア目標となる食べた団子数")]
    private int targetDangoCount;
    [SerializeField, Tooltip("現在の食べた団子数")]
    private int eatenDangoCount;
    [SerializeField]
    private GameMode gameMode = GameMode.Normal;

    private StageGenerator stageGenerator;
    private GameplayController gameplayController;
    private GameplayUIController gameplayUI;
    private ResultUIController resultUI;

    private List<SkewerController> skewers;

    [Tooltip("現在のゲームモード")]
    public GameMode CurrentGameMode => gameMode;

    protected override void StateInit()
    {
        // ステージのセットアップ
        StageData data = StageManager.Instance.CurrentStageData;
        stageGenerator = FindAnyObjectByType<StageGenerator>();
        skewers = stageGenerator.Generate(data);                // ステージ生成
        targetDangoCount = data.targetDangoCount;               // 目標数のセット
        // ゲームプレイUIの初期設定
        gameplayUI = FindAnyObjectByType<GameplayUIController>();
        gameplayUI.UpdateEatenDangoCount(eatenDangoCount, targetDangoCount);
        // リザルトUIの初期設定
        resultUI = FindAnyObjectByType<ResultUIController>();
        resultUI.onNextClicked += HandleNextClicked;
        resultUI.Hide();

        gameplayController = FindAnyObjectByType<GameplayController>();
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
                else if (IsGameOver()) Debug.Log("ゲームオーバー！！");
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
        return !gameplayController.CanUseEatAction() && eatenDangoCount < targetDangoCount;
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
        gameplayController.SetInputLocked(true);
    }
    /// <summary>
    /// ゲームを再開   </summary>
    public void ResumeGame()
    {
        gameplayController.SetInputLocked(false);
    }

    /// <summary>
    /// ステージ初期化    </summary>
    public void ResetStage()
    {
        // 串と団子を全削除
        Dango[] dangos = FindObjectsByType<Dango>(FindObjectsSortMode.None);
        foreach (Dango dango in dangos) Destroy(dango.gameObject);
        foreach(SkewerController skewer in skewers) Destroy(skewer.gameObject);
        // ステージ再生成
        skewers = stageGenerator.Generate(StageManager.Instance.CurrentStageData);
        // 目標数の初期化
        eatenDangoCount = 0;
        gameplayUI.UpdateEatenDangoCount(eatenDangoCount, targetDangoCount);
    }
    /// <summary>
    /// 次のステージをロード    </summary>
    public void LoadNextStage()
    {
        StageManager.Instance.GoToNextStage();
        ChangeScene(SceneType.Gameplay, true, sceneName);
    }
    /// <summary>
    /// 前のステージをロード    </summary>
    public void LoadPreviousStage()
    {
        StageManager.Instance.GoToPreviousStage();
        ChangeScene(SceneType.Gameplay, true, sceneName);
    }

    /// <summary>
    /// 団子を食べた際のイベント    </summary>
    /// <param name="eatenCount">
    /// 食べた個数    </param>
    public void OnDangoEaten(int eatenCount)
    {
        eatenDangoCount += eatenCount;
        gameplayUI.UpdateEatenDangoCount(eatenDangoCount, targetDangoCount);
    }
}
