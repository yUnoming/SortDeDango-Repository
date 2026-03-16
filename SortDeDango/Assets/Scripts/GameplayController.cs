using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField, Tooltip("ステージ初期化キー")]
    private KeyCode StageResetKey = KeyCode.R;

    private static GameplayController instance;
    public static GameplayController Instance { get { return instance; } }

    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if(Input.GetKeyDown(StageResetKey)) GameplayManager.Instance.ResetStage();
    }

    /// <summary>
    /// 串が選択された際のイベント    </summary>
    /// <param name="skewer">
    /// 選択された串    </param>
    public void OnSelectedSkewer(SkewerController skewer)
    {
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
            skewer.AddDango(selectingSkewer.GetTopDango());
            selectingSkewer.RemoveTopDango();
        }

        selectingSkewer.OnDeselect();
        selectingSkewer = null;
    }
}