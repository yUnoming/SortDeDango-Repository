using System.Collections;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField, Tooltip("ステージ初期化キー")]
    private KeyCode stageResetKey = KeyCode.R;
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

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
#if UNITY_EDITOR
        // ステージリセット
        if(Input.GetKeyDown(stageResetKey)) GameplayManager.Instance.ResetStage();
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

        Dango movingDango = from.RemoveTopDango();
        to.AddDango(movingDango);

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
    /// 串が選択された際のイベント    </summary>
    /// <param name="skewer">
    /// 選択された串    </param>
    public void OnSelectedSkewer(SkewerController skewer)
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

        selectingSkewer.OnDeselect();
        selectingSkewer = null;
    }
}