using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField, Tooltip("ステージリセットキー")]
    private KeyCode resetKey = KeyCode.R;
    [SerializeField, Tooltip("一手戻すキー")]
    private KeyCode undoKey = KeyCode.U;
    [SerializeField, Tooltip("ステージを進めるキー")]
    private KeyCode nextStageKey = KeyCode.RightArrow;
    [SerializeField, Tooltip("ステージを戻すキー")]
    private KeyCode previousStageKey = KeyCode.LeftArrow;

    private static GameplayController instance;
    public static GameplayController Instance { get { return instance; } }

    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;
    [Tooltip("入力制限中かどうか")]
    private bool isInputLocked;
    [Tooltip("移動データリスト")]
    private List<MoveData> moveDataList = new List<MoveData>();

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
#if UNITY_EDITOR
        // ステージリセット
        if(Input.GetKeyDown(resetKey)) GameplayManager.Instance.ResetStage();
        // 一手戻す
        if(Input.GetKeyDown(undoKey)) Undo();
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
    /// 一手戻す    </summary>
    public void Undo()
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