using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameplayController : MonoBehaviour
{
    [SerializeField, Tooltip("ステージリセットキー")]
    private KeyCode resetKey = KeyCode.R;
    [SerializeField, Tooltip("一手戻すキー")]
    private KeyCode undoKey = KeyCode.U;
    [SerializeField, Tooltip("食べるアクション用の入力キー")]
    private KeyCode eatKey = KeyCode.E;
    [SerializeField, Tooltip("ステージを進めるキー")]
    private KeyCode nextStageKey = KeyCode.RightArrow;
    [SerializeField, Tooltip("ステージを戻すキー")]
    private KeyCode previousStageKey = KeyCode.LeftArrow;
    [SerializeField]
    private EatModule eatModule;
    [SerializeField]
    private AudioData invalidActionSE;

    GameplayUIController gameplayUI;

    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;
    [Tooltip("入力制限中かどうか")]
    private bool isInputLocked;
    [Tooltip("食べる状態かどうか")]
    private bool isEatMode;
    [Tooltip("行動履歴")]
    private List<ActionLog> actionLogs = new List<ActionLog>();
    [Tooltip("手数")]
    private int moveCount;
    public int MoveCount => moveCount;

    private void Start()
    {
        // ゲームプレイUIへのイベント設定・表示セット
        gameplayUI = FindAnyObjectByType<GameplayUIController>();
        gameplayUI.onRestartClicked += HandleRestartClicked;
        gameplayUI.onUndoClicked += HandleUndoClicked;
        gameplayUI.onEatClicked += HandleEatClicked;
        gameplayUI.UpdateStageNumber(StageManager.Instance.CurrentStageNumber);
        gameplayUI.UpdateEatActionCount(
            eatModule.RemainingEatActionCount,
            eatModule.MaxEatActionCount);
        gameplayUI.UpdateMoveCount(moveCount);
    }
    private void Update()
    {
#if UNITY_EDITOR
        // ステージリセット
        if(Input.GetKeyDown(resetKey)) HandleRestartClicked();
        // 一手戻す
        if(Input.GetKeyDown(undoKey)) HandleUndoClicked();
        // 食べるアクション
        if (Input.GetKeyDown(eatKey)) HandleEatClicked();

        // ステージ進行・後退
        if(Input.GetKeyDown(nextStageKey)) GameplayManager.Instance.LoadNextStage();
        else if (Input.GetKeyDown(previousStageKey)) GameplayManager.Instance.LoadPreviousStage();
#endif
        // 左クリックの判定
        if(Input.GetMouseButtonDown(0)) HandleLeftClick();
    }

    /// <summary>
    /// 団子の移動制御    </summary>
    /// <param name="from">
    /// 移動元の串    </param>
    /// <param name="to">
    /// 移動先の串    </param>
    private IEnumerator MoveDangoSequence(SkewerController from, SkewerController to)
    {
        SetInputLocked(true);

        // 合計移動回数を取得し、その数だけ移動処理
        List<int> matchingDangoIndices = from.GetMatchingDangoIndices(from.GetTopDango());
        List<Dango> movedDangos = new List<Dango>();
        int totalMoveCount = to.RemainingDangoAddCount < matchingDangoIndices.Count
            ? to.RemainingDangoAddCount
            : matchingDangoIndices.Count;
        for (int moveCount = 0; moveCount < totalMoveCount ; moveCount++)
        {
            Dango movedDango = from.RemoveDangoAt(matchingDangoIndices[moveCount], false);
            to.AddDango(movedDango);
            movedDangos.Add(movedDango);

            // 最後に移動させる団子のアニメーション終了まで待機
            if (moveCount + 1 == totalMoveCount)
            {
                yield return movedDango.GetComponent<DangoMoveAnimation>().PlayCoroutine(
                    from,
                    to,
                    moveCount,
                    to.GetTopDangoPosition());
            }
            // それ以外
            else
            {
                movedDango.GetComponent<DangoMoveAnimation>().Play(
                    from,
                    to,
                    moveCount,
                    to.GetTopDangoPosition());
            }
        }
        // 行動履歴の保存
        MoveLog moveLog = new MoveLog(from, to, movedDangos);
        actionLogs.Add(moveLog);
        // 手数を増やして表示
        ++moveCount;
        gameplayUI.UpdateMoveCount(moveCount);

        DeselectCurrentSkewer();
        SetInputLocked(false);
    }
    /// <summary>
    /// 左クリック時の処理    </summary>
    private void HandleLeftClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // クリック位置へRaycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // 対象オブジェクトのクリック
            if (hit.collider != null)
            {
                // 団子選択の判定
                Dango dango = hit.collider.GetComponent<Dango>();
                if (dango != null)
                {
                    OnDangoClicked(dango);
                    return;
                }
                // 串選択の判定
                SkewerController skewer = hit.collider.GetComponent<SkewerController>();
                if (skewer != null)
                {
                    OnSkewerClicked(skewer);
                    return;
                }
            }
        }
        // 空クリックor対象外オブジェクトのクリック
        DeselectCurrentSelection();
    }
    /// <summary>
    /// 現在の選択を解除    </summary>
    private void DeselectCurrentSelection()
    {
        if (isEatMode) eatModule.CancelEat();
        else if(!isInputLocked && selectingSkewer != null) DeselectCurrentSkewer();
    }
    /// <summary>
    /// 新たな串の選択    </summary>
    /// <param name="skewer"></param>
    private void SelectCurrentSekewer(SkewerController skewer)
    {
        selectingSkewer = skewer;
        selectingSkewer.OnSelect();
    }
    /// <summary>
    /// 現在選択している串の選択解除    </summary>
    private void DeselectCurrentSkewer()
    {
        selectingSkewer.OnDeselect();
        selectingSkewer = null;
    }
    /// <summary>
    /// Restartボタン押下時の処理    </summary>
    private void HandleRestartClicked()
    {
        if (GameplayManager.currentState == SceneState.Running)
        {
            GameplayManager.Instance.ResetStage();
            Initialize();
        }
    }
    /// <summary>
    /// Undoボタン押下時の処理    </summary>
    private void HandleUndoClicked()
    {
        if (!isInputLocked)
            Undo();
    }
    /// <summary>
    /// Eatボタン押下時の処理    </summary>
    private void HandleEatClicked()
    {
        // 食べるモード実行中ならキャンセル
        if (isEatMode) eatModule.CancelEat();
        // 食べられる状態なら食べるモードへ以降
        else if (!isInputLocked && eatModule.CanEat())
            StartCoroutine(EatDangoSequence());
    }
    /// <summary>
    /// 一手戻す    </summary>
    private void Undo()
    {
        if (isInputLocked || actionLogs.Count == 0) return;

        // 直近の行動履歴を元に適切な戻す処理を行う
        ActionLog lastLog = actionLogs[actionLogs.Count - 1];
        switch (lastLog.type)
        {
            case ActionType.Move:
                {
                    MoveLog lastMoveLog = (MoveLog)lastLog;
                    for (int movedCount = lastMoveLog.movedDangos.Count; movedCount > 0; movedCount--)
                    {
                        Dango movedDango = lastMoveLog.to.RemoveTopDango();
                        lastMoveLog.from.AddDango(movedDango);
                        lastMoveLog.from.SetTopDangoPosition(movedDango);
                    }
                    --moveCount;
                    gameplayUI.UpdateMoveCount(moveCount);
                    break;
                }
            case ActionType.Eat:
                {
                    EatLog lastEatLog = (EatLog)lastLog;
                    int eatenCount = lastEatLog.eatenDangos.Count;
                    for (int index = eatenCount; index > 0; index--)
                    {
                        lastEatLog.skewer.AddDangoAt(
                            lastEatLog.eatenDangos[index - 1],
                            lastEatLog.matchingDangoIndices[index - 1]);
                    }
                    eatModule.OnUndoEaten(eatenCount);
                    gameplayUI.UpdateEatActionCount(
                        eatModule.RemainingEatActionCount,
                        eatModule.MaxEatActionCount);
                    gameplayUI.UpdateEatModeUI(isEatMode, eatModule.RemainingEatActionCount);
                }
                break;
        }
        // 使用済みの行動履歴を除外
        actionLogs.RemoveAt(actionLogs.Count - 1);
    }
    /// <summary>
    /// 団子を食べる    </summary>
    private IEnumerator EatDangoSequence()
    {
        SetInputLocked(true);
        SetEatModeActive(true);
        SetAllDangoOutlineVisible(true);

        yield return eatModule.EatSequence();
        gameplayUI.UpdateEatActionCount(
                eatModule.RemainingEatActionCount,
                eatModule.MaxEatActionCount
            );

        actionLogs.Add(eatModule.lastEatLog);   // 行動履歴を保存
        SetEatModeActive(false);
        SetAllDangoOutlineVisible(false);
        SetInputLocked(false);
    }
    /// <summary>
    /// 全団子のアウトライン表示切り替え    </summary>
    /// <param name="isVisible">
    /// 表示するかどうか    </param>
    private void SetAllDangoOutlineVisible(bool isVisible)
    {
        Dango[] dangos = FindObjectsByType<Dango>(FindObjectsSortMode.None);
        foreach (Dango dango in dangos) dango.SetOutlineVisible(isVisible);
    }

    /// <summary>
    /// 初期化    </summary>
    public void Initialize()
    {
        // 行動履歴
        actionLogs.Clear();
        moveCount = 0;
        // 食べるアクション・UI
        eatModule.Initialize();
        gameplayUI.UpdateEatActionCount(
            eatModule.RemainingEatActionCount,
            eatModule.MaxEatActionCount);
        gameplayUI.UpdateMoveCount(moveCount);
        // ゲームの状態
        SetInputLocked(false);
        SetEatModeActive(false);
        SetAllDangoOutlineVisible(false);
    }
    /// <summary>
    /// 食べるアクションを使用可能かどうか    </summary>
    public bool CanUseEatAction()
    {
        return eatModule.RemainingEatActionCount > 0;
    }
    /// <summary>
    /// 入力状態の設定    </summary>
    public void SetInputLocked(bool isLocked)
    {
        isInputLocked = isLocked;
    }
    /// <summary>
    /// 食べる状態の切り替え    </summary>
    public void SetEatModeActive(bool isActive)
    {
        isEatMode = isActive;
        gameplayUI.UpdateEatModeUI(isEatMode, eatModule.RemainingEatActionCount);
    }
    /// <summary>
    /// 串がクリックされた際のイベント    </summary>
    /// <param name="clickedSkewer">
    /// クリックされた串    </param>
    private void OnSkewerClicked(SkewerController clickedSkewer)
    {
        if (isInputLocked) return;

        // 現在選択中の串がなければ、今回クリックされた串を保持
        if (selectingSkewer == null)
        {
            if (clickedSkewer.CanSelect()) SelectCurrentSekewer(clickedSkewer);
            else
            {
                clickedSkewer.GetComponent<InvalidActionAnimation>().Play();
                AudioManager.Instance.PlaySE(invalidActionSE, false);
            }
            return;
        }
        // 保持中の串とは別の串をクリックした
        else if (clickedSkewer != selectingSkewer)
        {
            // クリックされた串へ団子が移動可能なら、移動処理へ
            if(clickedSkewer.CanMoveDango(selectingSkewer))
            {
                StartCoroutine(MoveDangoSequence(
                    selectingSkewer,
                    clickedSkewer));
            }
            // 移動不可なら、クリックされた串を選択対象とする
            else if (clickedSkewer.CanSelect())
            {
                DeselectCurrentSkewer();
                SelectCurrentSekewer(clickedSkewer);
            }
            else
            {
                clickedSkewer.GetComponent<InvalidActionAnimation>().Play();
                AudioManager.Instance.PlaySE(invalidActionSE, false);
                DeselectCurrentSkewer();
            }
            return;
        }

        DeselectCurrentSkewer();
    }
    /// <summary>
    /// 団子がクリックされた際のイベント    </summary>
    /// <param name="clickedDango">
    /// 選択された団子 </param>
    private void OnDangoClicked(Dango clickedDango)
    {
        // 食べる状態中
        if (isEatMode) eatModule.SetTargetDango(clickedDango);
        // 通常状態
        else OnSkewerClicked(clickedDango.CurrentSkewer);
    }
}