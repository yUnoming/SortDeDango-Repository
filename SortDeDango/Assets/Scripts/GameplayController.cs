using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private static GameplayController instance;
    public static GameplayController Instance { get { return instance; } }

    GameplayUIController gameplayUI;

    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;
    [Tooltip("入力制限中かどうか")]
    private bool isInputLocked;
    [Tooltip("移動データリスト")]
    private List<MoveData> moveDataList = new List<MoveData>();
    [Tooltip("手数")]
    private int moveCount;

    private void Start()
    {
        instance = this;

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
        if (Input.GetKeyDown(eatKey) && !isInputLocked)
        {
            if (eatModule.CanEat(selectingSkewer)) StartCoroutine(EatDangoSequence());
        }

        // ステージ進行・後退
        if(Input.GetKeyDown(nextStageKey)) GameplayManager.Instance.LoadNextStage();
        else if (Input.GetKeyDown(previousStageKey)) GameplayManager.Instance.LoadPreviousStage();
#endif
    }

    /// <summary>
    /// 団子の移動制御    </summary>
    /// <param name="from">
    /// 移動元の串    </param>
    /// <param name="to">
    /// 移動先の串    </param>
    private IEnumerator MoveDangoSequence(SkewerController from, SkewerController to)
    {
        isInputLocked = true;
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
        // 串の選択状態を解除
        from.OnDeselect();
        selectingSkewer = null;

        isInputLocked = false;
    }

    /// <summary>
    /// Restartボタン押下時の処理    </summary>
    private void HandleRestartClicked()
    {
        if (!isInputLocked)
            GameplayManager.Instance.ResetStage();
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
        if (!isInputLocked && eatModule.CanEat(selectingSkewer))
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
        isInputLocked = true;

        yield return eatModule.EatSequence(selectingSkewer);
        gameplayUI.UpdateEatActionCount(
                eatModule.RemainingEatActionCount,
                eatModule.MaxEatActionCount
            );
        selectingSkewer.OnDeselect();
        selectingSkewer = null;

        isInputLocked = false;
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
    /// 串が選択された際のイベント    </summary>
    /// <param name="skewer">
    /// 選択された串    </param>
    public void OnSkewerSelected(SkewerController skewer)
    {
        if (isInputLocked) return;

        // 既に選択されていなければ、今回選択された串を保持
        if (selectingSkewer == null)
        {
            selectingSkewer = skewer;
            selectingSkewer.OnSelect();
            return;
        }
        // 保持中の串とは別の串を選択したら、移動処理へ
        else if (skewer != selectingSkewer)
        {
            StartCoroutine(MoveDangoSequence(
                    selectingSkewer,
                    skewer
                ));
            return;
        }

        // 串の選択状態を解除
        selectingSkewer.OnDeselect();
        selectingSkewer = null;
    }
}