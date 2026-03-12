using UnityEditor.Profiling;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField]
    private SkewerController skewer1;
    [SerializeField]
    private SkewerController skewer2;

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            skewer2.AddDango(skewer1.GetTopDango());
            skewer1.RemoveTopDango();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            skewer1.AddDango(skewer2.GetTopDango());
            skewer2.RemoveTopDango();
        }
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
        /* 既に選択された串が保持している串と違ったら、
         * 団子移動処理へ  */
        else if(selectingSkewer != skewer)
        {
            skewer.AddDango(selectingSkewer.GetTopDango());
            selectingSkewer.RemoveTopDango();
        }

        selectingSkewer.OnDeselect();
        selectingSkewer = null;
    }
}