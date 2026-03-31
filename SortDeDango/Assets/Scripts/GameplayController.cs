using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    GameplayUIController gameplayUI;

    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;
    [Tooltip("入力制限中かどうか")]
    private bool isInputLocked;
    [Tooltip("食べる状態かどうか")]
    private bool isEatMode;
    [Tooltip("移動データリスト")]
    private List<MoveData> moveDataList = new List<MoveData>();
    [Tooltip("手数")]
    private int moveCount;

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

        // 移動データを作成
        MoveData moveData = new MoveData(from, to);
        // 移動させられるだけ、揃っている団子を全て移動
        for (int index = from.CurrentDangoCount; index > 0; index--)
        {
            if(to.CanMoveDango(from))
            {
                Dango movingDango = from.RemoveTopDango();
                to.AddDango(movingDango);
                moveData.dangoList.Add(movingDango);
                // 移動アニメーション
                yield return movingDango.GetComponent<DangoMoveAnimator>()
                    .PlayAnimation(
                        movingDango.transform,
                        to.GetTopDangoPosition()
                    );
            }
        }
        // 作成した移動データを追加
        moveDataList.Add(moveData);
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
        else if(selectingSkewer != null) DeselectCurrentSkewer();
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
        if (!isInputLocked && eatModule.CanEat())
            StartCoroutine(EatDangoSequence());
    }
    /// <summary>
    /// 一手戻す    </summary>
    private void Undo()
    {
        if (isInputLocked || moveDataList.Count == 0) return;

        // 最終移動データを元に、団子を元の串へ戻す
        MoveData lastData = moveDataList[moveDataList.Count - 1];
        for(int movedCount = lastData.dangoList.Count; movedCount > 0; movedCount--)
        {
            Dango movedDango = lastData.to.RemoveTopDango();
            lastData.from.AddDango(movedDango);
            lastData.from.SetTopDangoPosition(movedDango);
        }
        // 使用済みの移動データを除外
        moveDataList.RemoveAt(moveDataList.Count - 1);
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

        SetInputLocked(false);
        SetEatModeActive(false);
        SetAllDangoOutlineVisible(false);
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
        // 移動データ
        moveDataList.Clear();
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
            SelectCurrentSekewer(clickedSkewer);
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
            else
            {
                DeselectCurrentSkewer();
                SelectCurrentSekewer(clickedSkewer);
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