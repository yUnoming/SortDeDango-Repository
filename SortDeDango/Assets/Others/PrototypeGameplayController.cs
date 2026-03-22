using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeGameplayController : MonoBehaviour
{
    [SerializeField, Tooltip("ステージリセットキー")]
    private KeyCode resetKey = KeyCode.R;
    [SerializeField, Tooltip("一手戻すキー")]
    private KeyCode undoKey = KeyCode.U;
    [SerializeField, Tooltip("食べるアクション入力キー")]
    private KeyCode eatKey = KeyCode.E;
    [SerializeField, Tooltip("串生成アクション入力キー")]
    private KeyCode generateKey = KeyCode.G;
    [SerializeField, Tooltip("ステージを進めるキー")]
    private KeyCode nextStageKey = KeyCode.RightArrow;
    [SerializeField, Tooltip("ステージを戻すキー")]
    private KeyCode previousStageKey = KeyCode.LeftArrow;

    private static PrototypeGameplayController instance;
    public static PrototypeGameplayController Instance { get { return instance; } }

    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;
    [Tooltip("入力制限中かどうか")]
    private bool isInputLocked;
    [Tooltip("移動データリスト")]
    private List<MoveData> moveDataList = new List<MoveData>();

    private int totalSkewers = 5;
    private int currentSkewers = 1;
    private int requiredCompleteSkewer = 3;
    private int completeSkewerCount;
    private int generateMoveNumber = 3;         // 生成に必要な手数
    private int generateRemainMoveNumber = 5;   // 生成までの残り手数

    private void Start()
    {
        instance = this;

        // ゲームプレイUIへのイベント設定・ステージ番号設定
        GameplayUIController gameplayUI = FindAnyObjectByType<GameplayUIController>();
        gameplayUI.onRestartClicked += HandleRestartClicked;
        gameplayUI.onUndoClicked += HandleUndoClicked;
        gameplayUI.SetStageNumber(StageManager.Instance.CurrentStageNumber);
    }
    private void Update()
    {
#if UNITY_EDITOR
        //=====
        // 通常アクション
        // ステージリセット
        if (Input.GetKeyDown(resetKey)) HandleRestartClicked();
        // 一手戻す
        if (Input.GetKeyDown(undoKey)) HandleUndoClicked();
        // ステージ進行・後退
        if (Input.GetKeyDown(nextStageKey)) GameplayManager.Instance.LoadNextStage();
        else if (Input.GetKeyDown(previousStageKey)) GameplayManager.Instance.LoadPreviousStage();

        //=====
        // プロトタイプアクション
        // 団子をだべる
        if(Input.GetKeyDown(eatKey) && selectingSkewer) EatTopDano();
        // 新たな串生成
        if (Input.GetKeyDown(generateKey))
        {
            GenerateSkewer();
            generateRemainMoveNumber--;
        }
        if(generateRemainMoveNumber <= 0)
        {
            GenerateSkewer();
            generateRemainMoveNumber = generateMoveNumber;
        }

        if (completeSkewerCount >= requiredCompleteSkewer) Debug.Log("ゲームクリア！！");
        else if (currentSkewers >= totalSkewers) Debug.Log("ゲームオーバー！！");

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

        // 団子を移動
        Dango movingDango = from.RemoveTopDango();
        to.AddDango(movingDango);
        // 移動データを追加
        moveDataList.Add(new MoveData(from, to, movingDango));
        // 串の選択状態を解除
        from.OnDeselect();
        selectingSkewer = null;

        yield return movingDango.GetComponent<DangoMoveAnimator>()
            .PlayAnimation(
                movingDango.transform,
                to.GetTopDangoPosition()
            );

        isInputLocked = false;
    }

    /// <summary>
    /// Restartボタン押下時の処理    </summary>
    private void HandleRestartClicked()
    {
        GameplayManager.Instance.ResetStage();
    }
    /// <summary>
    /// Undoボタン押下時の処理    </summary>
    private void HandleUndoClicked()
    {
        Undo();
    }
    /// <summary>
    /// 一手戻す    </summary>
    private void Undo()
    {
        if (isInputLocked || moveDataList.Count == 0) return;

        // 最終移動データを元に、団子を元の串へ戻す
        MoveData lastData = moveDataList[moveDataList.Count - 1];
        Dango movedDango = lastData.to.RemoveTopDango();
        lastData.from.AddDango(movedDango);
        lastData.from.SetTopDangoPosition(movedDango);
        // 使用済みの移動データを除外
        moveDataList.RemoveAt(moveDataList.Count - 1);
    }
    /// <summary>
    /// 選択中の串の一番上にある団子を食べる    </summary>
    private void EatTopDano()
    {
        Dango removedDango = selectingSkewer.RemoveTopDango();
        if(removedDango != null)
        {
            Destroy(removedDango.gameObject);
            generateRemainMoveNumber--;
        }
        selectingSkewer.OnDeselect();
        selectingSkewer = null;
    }
    /// <summary>
    /// 新たな串を生成    </summary>
    private void GenerateSkewer()
    {
        StageData stageData = new StageData();
        stageData.totalSkewers = 1;
        stageData.dangoLists = new List<DangoList>
        {
            new DangoList()
        };

        int dangoCount = UnityEngine.Random.Range(1, 4);
        for(int dangoAddCount = 0; dangoAddCount <  dangoCount; dangoAddCount++)
        {
            int dangoColor = UnityEngine.Random.Range(1, 4);
            stageData.dangoLists[0].dangoColors.Add((DangoColor)dangoColor);
        }

        StageGenerator stageGenerator = FindAnyObjectByType<StageGenerator>();
        stageGenerator.Generate(stageData);

        currentSkewers++;
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
        // 団子が移動可能であれば、団子移動処理へ
        else if (skewer.CanMoveDango(selectingSkewer))
        {
            generateRemainMoveNumber--;

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
    /// <summary>
    /// 串が完成した際のイベント    </summary>
    /// <param name="skewer">
    /// 完成した串    </param>
    public void OnSkewerCompleted(SkewerController skewer)
    {
        isInputLocked = false;

        completeSkewerCount++;
        currentSkewers--;
        Destroy(skewer.gameObject);
    }
    /// <summary>
    /// 串が空になった際のイベント    </summary>
    /// <param name="skewer">
    /// 空になった串    </param>
    public void OnSkewerEmptied(SkewerController skewer)
    {
        currentSkewers--;
        Destroy(skewer.gameObject);
    }
}